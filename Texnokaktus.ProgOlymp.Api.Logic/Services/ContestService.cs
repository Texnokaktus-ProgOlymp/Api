using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.Api.DataAccess.Context;
using Texnokaktus.ProgOlymp.Api.DataAccess.Entities;
using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services;

internal class ContestService(AppDbContext context,
                              IContestDataServiceClient contestDataServiceClient,
                              IParticipantServiceClient participantServiceClient,
                              TimeProvider timeProvider) : IContestService
{
    public async Task<int> AddContestAsync(string name,
                                           string title,
                                           DateTimeOffset registrationStart,
                                           DateTimeOffset registrationFinish,
                                           long? preliminaryStageId,
                                           long? finalStageId)
    {
        var contest = new Contest
        {
            Name = name,
            Title = title,
            RegistrationStart = registrationStart,
            RegistrationFinish = registrationFinish
        };

        if (preliminaryStageId.HasValue)
            contest.PreliminaryStage = await GetContestStageAsync(preliminaryStageId.Value, CancellationToken.None);

        if (finalStageId.HasValue)
            contest.FinalStage = await GetContestStageAsync(finalStageId.Value, CancellationToken.None);

        context.Contests.Add(contest);

        await context.SaveChangesAsync();

        return contest.Id;
    }

    private async Task<ContestStage> GetContestStageAsync(long contestStageId, CancellationToken cancellationToken)
    {
        var description = await contestDataServiceClient.GetContestAsync(contestStageId, cancellationToken);

        var stage = new ContestStage
        {
            ContestId = contestStageId,
            Name = description.Name,
            ContestStart = description.StartTime.ToDateTimeOffset(),
            Duration = description.Duration.ToTimeSpan()
        };

        if (await participantServiceClient.GetContestOwnerParticipationAsync(contestStageId, cancellationToken) is { } participantStatus)
        {
            stage.ContestFinish = participantStatus.ContestFinishTime?.ToDateTimeOffset();
        }

        return stage;
    }

    public Task<Domain.Contest?> GetContestAsync(string contestName) =>
        context.Contests
               .Include(contest => contest.PreliminaryStage)
               .Include(contest => contest.FinalStage)
               .Where(contest => contest.Name == contestName)
               .Select(contest => contest.MapContest(timeProvider.GetUtcNow()))
               .FirstOrDefaultAsync();
}

file static class MappingExtensions
{
    [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
    private static Domain.RegistrationState GetState(Contest contest, DateTimeOffset now)
    {
        if (contest.PreliminaryStage is null)
            return Domain.RegistrationState.Unavailable;

        if (now < contest.RegistrationStart) return Domain.RegistrationState.NotStarted;
        if (now >= contest.RegistrationFinish) return Domain.RegistrationState.Finished;
        return Domain.RegistrationState.InProgress;
    }

    public static Domain.Contest MapContest(this Contest contest, DateTimeOffset now) =>
        new()
        {
            Id = contest.Id,
            Name = contest.Name,
            RegistrationStart = contest.RegistrationStart,
            RegistrationFinish = contest.RegistrationFinish,
            PreliminaryStage = contest.PreliminaryStage?.MapContestStage(),
            FinalStage = contest.FinalStage?.MapContestStage(),
            RegistrationState = GetState(contest, now)
        };

    private static Domain.ContestStage MapContestStage(this ContestStage contestStage) =>
        new()
        {
            Id = contestStage.ContestId,
            Name = contestStage.Name,
            ContestStart = contestStage.ContestStart,
            ContestFinish = contestStage.ContestFinish,
            Duration = contestStage.Duration
        };
}
