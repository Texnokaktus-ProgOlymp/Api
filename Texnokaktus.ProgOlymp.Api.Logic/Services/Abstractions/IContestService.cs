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

    Task<Contest?> GetContestAsync(int id);
    async Task<Contest> GetRequiredContestAsync(int id) => await GetContestAsync(id) ?? throw new ContestNotFoundException(id);
}
