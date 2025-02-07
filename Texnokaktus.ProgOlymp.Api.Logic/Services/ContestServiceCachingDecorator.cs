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
        memoryCache.Remove(GetKey(id));

        return id;
    }

    public Task<Contest?> GetContestAsync(int id) =>
        memoryCache.GetOrCreateAsync(GetKey(id),
                                     entry =>
                                     {
                                         entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                                         return contestService.GetContestAsync(id);
                                     });

    private static string GetKey(int contestId) => $"Contests:{contestId}";
}
