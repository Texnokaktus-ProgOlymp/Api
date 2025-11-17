using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Logic.Exceptions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

public interface IContestService
{
    Task<int> AddContestAsync(string name,
                              DateTimeOffset registrationStart,
                              DateTimeOffset registrationFinish,
                              long? preliminaryStageId,
                              long? finalStageId);

    Task<Contest?> GetContestAsync(string contestName);

    async Task<Contest> GetRequiredContestAsync(string contestName) =>
        await GetContestAsync(contestName) ?? throw new ContestNotFoundException(contestName);
}
