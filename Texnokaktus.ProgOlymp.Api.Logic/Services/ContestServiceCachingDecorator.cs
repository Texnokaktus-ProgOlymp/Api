using Microsoft.Extensions.Caching.Memory;
using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services;

public class ContestServiceCachingDecorator(IContestService contestService, IMemoryCache memoryCache) : IContestService
{
    public async Task<int> AddContestAsync(string name,
                                           DateTimeOffset registrationStart,
                                           DateTimeOffset registrationFinish,
                                           long? preliminaryStageId,
                                           long? finalStageId)
    {
        var id = await contestService.AddContestAsync(name, registrationStart, registrationFinish, preliminaryStageId, finalStageId);
        memoryCache.Remove(GetKey(name));
        memoryCache.Remove(GetExistenceKey(name));

        return id;
    }

    public Task<Contest?> GetContestAsync(string contestName) =>
        memoryCache.GetOrCreateAsync(GetKey(contestName),
                                     entry =>
                                     {
                                         entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                                         return contestService.GetContestAsync(contestName);
                                     });

    public Task<bool> IsContestExistAsync(string contestName) =>
        memoryCache.GetOrCreateAsync(GetExistenceKey(contestName),
                                     entry =>
                                     {
                                         entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                                         return contestService.IsContestExistAsync(contestName);
                                     });

    private static string GetKey(string contestName) => $"Contests:{contestName}";
    private static string GetExistenceKey(string contestName) => $"Contests:{contestName}:Exists";
}
