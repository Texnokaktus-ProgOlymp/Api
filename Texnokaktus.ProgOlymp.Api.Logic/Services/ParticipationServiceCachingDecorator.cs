using Microsoft.Extensions.Caching.Memory;
using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services;

public class ParticipationServiceCachingDecorator(IParticipationService service, IMemoryCache memoryCache) : IParticipationService
{
    public async Task<ContestParticipation> GetContestParticipationAsync(int userId, string contestName)
    {
        if (memoryCache.Get<ContestParticipation>(GetKey(userId, contestName)) is { } contestParticipation)
            return contestParticipation;

        var participation = await service.GetContestParticipationAsync(userId, contestName);

        return participation.IsUserRegistered
                   ? memoryCache.Set(GetKey(userId, contestName), participation, TimeSpan.FromMinutes(2.5))
                   : participation;
    }

    private static string GetKey(int userId, string contestName) => $"Contests:{contestName}:Users:{userId}:Participation";
}
