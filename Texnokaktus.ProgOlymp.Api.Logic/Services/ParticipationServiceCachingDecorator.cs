using Microsoft.Extensions.Caching.Memory;
using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services;

public class ParticipationServiceCachingDecorator(IParticipationService service, IMemoryCache memoryCache) : IParticipationService
{
    public async Task<ContestParticipation> GetContestParticipationAsync(int userId, int contestId)
    {
        if (memoryCache.Get<ContestParticipation>(GetKey(userId, contestId)) is { } contestParticipation)
            return contestParticipation;

        var participation = await service.GetContestParticipationAsync(userId, contestId);

        return participation.IsUserRegistered
                   ? memoryCache.Set(GetKey(userId, contestId), participation, TimeSpan.FromMinutes(2.5))
                   : participation;
    }

    private static string GetKey(int userId, int contestId) => $"Contests:{contestId}:Users:{userId}:Participation";
}
