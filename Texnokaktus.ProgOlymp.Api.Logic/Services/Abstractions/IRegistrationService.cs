using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Logic.Models;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

public interface IRegistrationService
{
    Task<ContestRegistrationState?> GetRegistrationStateAsync(int contestId);
    Task<bool> IsUserRegisteredAsync(int contestId, int userId);
    Task<int> RegisterUserAsync(ApplicationInsertModel userInsertModel);
    Task<ContestApplications?> GetContestApplicationsAsync(int contestId);
}
