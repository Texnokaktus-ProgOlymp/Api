using Microsoft.AspNetCore.Http.HttpResults;

namespace Texnokaktus.ProgOlymp.Api.Services.Abstractions;

public interface IParticipationService
{
    Task<Results<Ok<Domain.ContestParticipation>, NotFound>> GetParticipationAsync(int userId, int contestId);
}
