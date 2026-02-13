using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Logic.Models;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

public interface IRegistrationService
{
    Task<bool> IsUserRegisteredAsync(string contestName, int userId);
    Task<int> RegisterUserAsync(ApplicationInsertModel userInsertModel);
    Task<ContestApplications?> GetContestApplicationsAsync(string contestName);
}
