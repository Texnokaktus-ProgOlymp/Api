using Microsoft.EntityFrameworkCore;
using Quartz;
using Texnokaktus.ProgOlymp.Api.DataAccess.Context;
using Texnokaktus.ProgOlymp.Api.DataAccess.Entities;
using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;

namespace Texnokaktus.ProgOlymp.Api.Logic.Jobs;

internal class ParticipationUpdateJob(AppDbContext dbContext,
                                      IParticipantServiceClient participantServiceClient,
                                      IContestDataServiceClient contestDataServiceClient,
                                      IResultServiceClient resultServiceClient,
                                      TimeProvider timeProvider) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await foreach (var contest in dbContext.Contests
                                               .Include(contest => contest.Applications)
                                               .AsAsyncEnumerable())
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (contest.PreliminaryStage is { } preliminaryStage)
                await UpdateContestStageAsync(contest.Name,
                                              Common.Contracts.Grpc.Results.ContestStage.Preliminary,
                                              preliminaryStage,
                                              contest.Applications,
                                              application => application.PreliminaryStageParticipation,
                                              context.CancellationToken);

            if (contest.FinalStage is { } finalStage)
                await UpdateContestStageAsync(contest.Name,
                                              Common.Contracts.Grpc.Results.ContestStage.Final,
                                              finalStage,
                                              contest.Applications,
                                              application => application.FinalStageParticipation,
                                              context.CancellationToken);
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }

    private async Task UpdateContestStageAsync(string contestName,
                                               Common.Contracts.Grpc.Results.ContestStage stageType,
                                               ContestStage stage,
                                               ICollection<Application> applications,
                                               Func<Application, Participation?> participationSelector,
                                               CancellationToken cancellationToken)
    {
        if (stage.State == ContestStageState.Finished) return;

        var ownerParticipation = await participantServiceClient.GetContestOwnerParticipationAsync(stage.YandexContestId, cancellationToken);

        var now = timeProvider.GetUtcNow();
        var contestStart = ownerParticipation.ContestStartTime.ToDateTimeOffset();
        var contestFinish = ownerParticipation.ContestFinishTime.ToDateTimeOffset();

        stage.ContestStart = contestStart;
        stage.ContestFinish = contestFinish;

        if (stage.State == ContestStageState.NotStarted && now >= stage.ContestStart)
            await StartContestStageAsync(contestName, stageType, stage, cancellationToken);

        var contestParticipants = await participantServiceClient.GetContestParticipantsAsync(stage.YandexContestId, cancellationToken);

        var inProgressParticipantsCount = 0;

        foreach (var arg in applications.Join(contestParticipants,
                                              application => participationSelector.Invoke(application)?.ContestUserId,
                                              participantInfo => participantInfo.Id,
                                              (application, info) => new
                                              {
                                                  Application = application,
                                                  ContestParticipantInfo = info
                                              }))
        {
            if (participationSelector.Invoke(arg.Application) is not { } participation) continue;
            if (participation.State == ParticipationState.Finished) continue;
            if (arg.ContestParticipantInfo.StartTime is null) continue;

            var participantStatus = await participantServiceClient.GetParticipantStatusAsync(stage.YandexContestId, participation.ContestUserId, cancellationToken);

            participation.Start = participantStatus.StartTime?.ToDateTimeOffset();
            participation.Finish = participantStatus.FinishTime?.ToDateTimeOffset();
            participation.State = participantStatus.State.MapParticipationState();
            
            if (participation.State == ParticipationState.InProgress)
                inProgressParticipantsCount++;
        }

        if (stage.State == ContestStageState.InProgress && now >= stage.ContestFinish && inProgressParticipantsCount == 0)
        {
            var participantIdDictionary = applications.Select(application => new
                                                       {
                                                           ApplicationId = application.Id,
                                                           participationSelector.Invoke(application)?.ContestUserId
                                                       })
                                                      .Where(arg => arg.ContestUserId.HasValue)
                                                      .ToDictionary(arg => arg.ContestUserId!.Value,
                                                                    arg => arg.ApplicationId);

            await FinishContestStageAsync(contestName, stageType, stage, participantIdDictionary, cancellationToken);
        }
    }

    private async Task StartContestStageAsync(string contestName, Common.Contracts.Grpc.Results.ContestStage stageType, ContestStage stage, CancellationToken cancellationToken)
    {
        stage.State = ContestStageState.InProgress;

        await resultServiceClient.AddContestAsync(contestName, stageType, stage.YandexContestId, cancellationToken);

        foreach (var problem in await contestDataServiceClient.GetContestProblemsAsync(stage.YandexContestId, cancellationToken))
            await resultServiceClient.AddProblemAsync(contestName, stageType, problem.Alias, problem.Name, cancellationToken);
    }

    private async Task FinishContestStageAsync(string contestName, Common.Contracts.Grpc.Results.ContestStage stageType, ContestStage stage, IReadOnlyDictionary<long, int> participantIdDictionary, CancellationToken cancellationToken)
    {
        stage.State = ContestStageState.Finished;

        var standings = await contestDataServiceClient.GetStandingsAsync(stage.YandexContestId, 1, 100, null, cancellationToken);

        foreach (var resultRow in standings.Rows
                                           .Select(row => new
                                            {
                                                row.ParticipantInfo,
                                                ProblemResults = standings.Titles.Zip(row.ProblemResults,
                                                                                      (title, result) => new
                                                                                      {
                                                                                          Problem = title,
                                                                                          Result = result
                                                                                      })
                                            }))
        foreach (var problemResult in resultRow.ProblemResults.Where(x => x.Result.Status == SubmissionStatus.Accepted))
        {
            if (participantIdDictionary.TryGetValue(resultRow.ParticipantInfo.Id, out var participantId) && problemResult.Result.Score is { } score)
            {
                await resultServiceClient.AddResultAsync(contestName,
                                                         stageType,
                                                         problemResult.Problem.Title,
                                                         participantId,
                                                         Convert.ToDecimal(score),
                                                         cancellationToken);
            }
        }
    }
}

file static class MappingExtensions
{
    public static ParticipationState MapParticipationState(this Common.Contracts.Grpc.Common.ParticipationState state) =>
        state switch
        {
            Common.Contracts.Grpc.Common.ParticipationState.NotStarted => ParticipationState.NotStarted,
            Common.Contracts.Grpc.Common.ParticipationState.InProgress => ParticipationState.InProgress,
            Common.Contracts.Grpc.Common.ParticipationState.Finished   => ParticipationState.Finished,
            _                                                          => throw new ArgumentOutOfRangeException(nameof(state), state, $"Unable to map {nameof(ParticipationState)}")
        };
}
