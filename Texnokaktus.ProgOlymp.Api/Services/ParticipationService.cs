using Microsoft.AspNetCore.Http.HttpResults;
using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Services;

public class ParticipationService(Logic.Services.Abstractions.IParticipationService participationService,
                                  Logic.Services.Abstractions.IContestService contestService) : IParticipationService
{
    public async Task<Results<Ok<ContestParticipation>, NotFound>> GetParticipationAsync(int userId, string contestName)
    {
        if (!await contestService.IsContestExistAsync(contestName))
            return TypedResults.NotFound();

        var participation = await participationService.GetContestParticipationAsync(userId, contestName);

        return TypedResults.Ok(participation);
    }
}
