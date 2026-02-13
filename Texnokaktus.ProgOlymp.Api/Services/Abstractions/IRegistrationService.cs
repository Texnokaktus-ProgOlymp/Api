using Microsoft.AspNetCore.Http.HttpResults;
using Texnokaktus.ProgOlymp.Api.Models;

namespace Texnokaktus.ProgOlymp.Api.Services.Abstractions;

public interface IRegistrationService
{
    Task<Results<Created, NotFound, Conflict>> RegisterUserAsync(string contestName, int userId, ApplicationInsertModel insertModel);
}
