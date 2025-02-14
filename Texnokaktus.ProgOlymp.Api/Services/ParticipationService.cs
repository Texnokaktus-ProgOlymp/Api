using Microsoft.AspNetCore.Http.HttpResults;
using Texnokaktus.ProgOlymp.Api.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Services;

public class ParticipationService(Logic.Services.Abstractions.IParticipationService participationService,
                                  Logic.Services.Abstractions.IContestService contestService) : IParticipationService
{
    public async Task<Results<Ok<Domain.ContestParticipation>, NotFound>> GetParticipationAsync(int userId, int contestId)
    {
        if (await contestService.GetContestAsync(contestId) is not { } contest)
            return TypedResults.NotFound();

        var participation = await participationService.GetContestParticipationAsync(userId, contestId);

        return TypedResults.Ok(participation);
    }
}
