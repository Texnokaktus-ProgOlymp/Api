using Microsoft.AspNetCore.Http.HttpResults;
using Texnokaktus.ProgOlymp.Api.Models;

namespace Texnokaktus.ProgOlymp.Api.Services.Abstractions;

public interface IRegistrationService
{
    Task<Results<Ok<ContestRegistrationState>, NotFound>> GetRegistrationStateAsync(int contestId);
    Task<Results<Created, NotFound, Conflict>> RegisterUserAsync(int contestId, int userId, ApplicationInsertModel insertModel);
}
