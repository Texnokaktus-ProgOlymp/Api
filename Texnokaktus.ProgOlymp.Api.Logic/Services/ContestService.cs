using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Texnokaktus.ProgOlymp.Api.DataAccess.Context;
using Texnokaktus.ProgOlymp.Api.DataAccess.Entities;
using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services;

internal class ContestService(AppDbContext context,
                              IContestDataServiceClient contestDataServiceClient,
                              IParticipantServiceClient participantServiceClient,
                              IMemoryCache memoryCache) : IContestService
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
        memoryCache.Remove(GetKey(name));
        memoryCache.Remove(GetExistenceKey(name));

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
        memoryCache.GetOrCreateAsync(GetKey(contestName),
                                     entry =>
                                     {
                                         entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                                         return context.Contests
                                                       .Include(contest => contest.PreliminaryStage)
                                                       .Include(contest => contest.FinalStage)
                                                       .Where(contest => contest.Name == contestName)
                                                       .Select(contest => contest.MapContest())
                                                       .FirstOrDefaultAsync();
                                     });

    public Task<bool> IsContestExistAsync(string contestName) =>
        memoryCache.GetOrCreateAsync(GetExistenceKey(contestName),
                                     entry =>
                                     {
                                         entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                                         return context.Contests.AnyAsync(contest => contest.Name == contestName);
                                     });

    private static string GetKey(string contestName) => $"Contests:{contestName}";
    private static string GetExistenceKey(string contestName) => $"Contests:{contestName}:Exists";
}

file static class MappingExtensions
{
    public static Domain.Contest MapContest(this Contest contest) =>
        new()
        {
            Id = contest.Id,
            Name = contest.Name,
            RegistrationStart = contest.RegistrationStart,
            RegistrationFinish = contest.RegistrationFinish,
            PreliminaryStage = contest.PreliminaryStage?.MapContestStage(),
            FinalStage = contest.FinalStage?.MapContestStage()
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
