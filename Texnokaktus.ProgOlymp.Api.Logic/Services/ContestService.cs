using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Texnokaktus.ProgOlymp.Api.DataAccess.Context;
using Texnokaktus.ProgOlymp.Api.DataAccess.Entities;
using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services;

internal class ContestService(AppDbContext context, IContestDataServiceClient contestDataServiceClient, IMemoryCache memoryCache) : IContestService
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
        {
            var description = await contestDataServiceClient.GetContestAsync(preliminaryStageId.Value);
            contest.PreliminaryStage = new()
            {
                ContestId = preliminaryStageId.Value,
                Name = description.Name,
                ContestStart = description.StartTime.ToDateTimeOffset(),
                Duration = description.Duration.ToTimeSpan()
            };
        }

        if (finalStageId.HasValue)
        {
            var description = await contestDataServiceClient.GetContestAsync(finalStageId.Value);
            contest.FinalStage = new()
            {
                ContestId = finalStageId.Value,
                Name = description.Name,
                ContestStart = description.StartTime.ToDateTimeOffset(),
                Duration = description.Duration.ToTimeSpan()
            };
        }

        await context.SaveChangesAsync();
        memoryCache.Remove(GetKey(name));
        memoryCache.Remove(GetExistenceKey(name));

        return contest.Id;
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
        new(contest.Id,
            contest.Name,
            contest.RegistrationStart,
            contest.RegistrationFinish,
            contest.PreliminaryStage?.MapContestStage(),
            contest.FinalStage?.MapContestStage());

    private static Domain.ContestStage MapContestStage(this ContestStage contestStage) =>
        new(contestStage.ContestId,
            contestStage.Name,
            contestStage.ContestStart,
            contestStage.ContestFinish,
            contestStage.Duration);
}
