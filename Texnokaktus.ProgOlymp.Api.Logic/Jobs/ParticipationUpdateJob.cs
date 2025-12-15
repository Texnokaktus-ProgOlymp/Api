using Microsoft.EntityFrameworkCore;
using Quartz;
using Texnokaktus.ProgOlymp.Api.DataAccess.Context;
using Texnokaktus.ProgOlymp.Api.DataAccess.Entities;
using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Jobs;

internal class ParticipationUpdateJob(AppDbContext dbContext, IParticipantServiceClient client) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await foreach (var contest in dbContext.Contests
                                               .Include(contest => contest.Applications)
                                               .AsAsyncEnumerable())
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (contest.PreliminaryStage is { } preliminaryStage)
                await UpdateContestStageAsync(preliminaryStage,
                                              contest.Applications,
                                              application => application.PreliminaryStageParticipation,
                                              context.CancellationToken);

            if (contest.FinalStage is { } finalStage)
                await UpdateContestStageAsync(finalStage,
                                              contest.Applications,
                                              application => application.FinalStageParticipation,
                                              context.CancellationToken);

            await dbContext.SaveChangesAsync(context.CancellationToken);
        }
    }

    private async Task UpdateContestStageAsync(ContestStage stage,
                                               IEnumerable<Application> applications,
                                               Func<Application, Participation?> participationSelector,
                                               CancellationToken cancellationToken)
    {
        var contestParticipants = await client.GetContestParticipantsAsync(stage.ContestId, cancellationToken);

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
            if (participation.State == ParticipationState.Finished) return;
            if (arg.ContestParticipantInfo.StartTime is null) return;

            var participantStatus = await client.GetParticipantStatusAsync(stage.ContestId, arg.Application.Id, cancellationToken);

            participation.Start = participantStatus.StartTime?.ToDateTimeOffset();
            participation.Finish = participantStatus.FinishTime?.ToDateTimeOffset();
            participation.State = participantStatus.State.MapParticipationState();
        }
    }
}

file static class MappingExtensions
{
    public static ParticipationState MapParticipationState(this Common.Contracts.Grpc.YandexContest.ParticipationState state) =>
        state switch
        {
            Common.Contracts.Grpc.YandexContest.ParticipationState.NotStarted => ParticipationState.NotStarted,
            Common.Contracts.Grpc.YandexContest.ParticipationState.InProgress => ParticipationState.InProgress,
            Common.Contracts.Grpc.YandexContest.ParticipationState.Finished   => ParticipationState.Finished,
            _                                                                 => throw new ArgumentOutOfRangeException(nameof(state), state, $"Unable to map {nameof(ParticipationState)}")
        };
}
