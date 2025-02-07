using Texnokaktus.ProgOlymp.Api.Domain;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

public interface IRegistrationService
{
    Task<ContestRegistrationState?> GetRegistrationState(int contestId);
    Task RegisterUserToPreliminaryStageAsync(int contestId, string login, string? displayName);
}
