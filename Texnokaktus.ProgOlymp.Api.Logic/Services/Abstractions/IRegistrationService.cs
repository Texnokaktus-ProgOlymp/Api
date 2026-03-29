using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Logic.Models;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

public interface IRegistrationService
{
    Task<bool> IsUserRegisteredAsync(string contestName, int userId);
    Task<int> RegisterUserAsync(ApplicationInsertModel userInsertModel);
    Task<ContestApplications?> GetContestApplicationsAsync(string contestName);
    Task<IEnumerable<KeyValuePair<Application, DataAccess.Entities.InternalUser>>> QualifyUsersAsync(string contestName, decimal scoreThreshold, CancellationToken cancellationToken);
    Task<IEnumerable<KeyValuePair<Application, DataAccess.Entities.InternalUser>>> QualifyUsersDryRunAsync(string contestName, decimal scoreThreshold, CancellationToken cancellationToken);
}
