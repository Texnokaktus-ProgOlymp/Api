using Microsoft.AspNetCore.Http.HttpResults;
using Texnokaktus.ProgOlymp.Api.Domain;

namespace Texnokaktus.ProgOlymp.Api.Services.Abstractions;

public interface IParticipationService
{
    Task<Results<Ok<ContestParticipation>, NotFound>> GetParticipationAsync(int userId, string contestName);
}
