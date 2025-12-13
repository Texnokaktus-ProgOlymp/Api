using Microsoft.Extensions.Caching.Memory;
using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services;

public class ParticipationServiceCachingDecorator(IParticipationService service, IMemoryCache memoryCache) : IParticipationService
{
    public async Task<ContestParticipation> GetContestParticipationAsync(int userId, string contestName) =>
        await memoryCache.GetOrCreateAsync(GetKey(userId, contestName),
                                           entry =>
                                           {
                                               entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2.5));
                                               return service.GetContestParticipationAsync(userId, contestName);
                                           })
     ?? new ContestParticipation(false, null, null);
    

    private static string GetKey(int userId, string contestName) => $"Contests:{contestName}:Users:{userId}:Participation";
}
