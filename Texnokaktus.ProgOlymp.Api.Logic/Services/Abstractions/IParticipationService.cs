using Texnokaktus.ProgOlymp.Api.Domain;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

public interface IParticipationService
{
    Task<ContestParticipation> GetContestParticipationAsync(int userId, string contestName);
}
