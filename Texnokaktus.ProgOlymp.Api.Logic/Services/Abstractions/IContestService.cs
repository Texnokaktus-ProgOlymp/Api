using Texnokaktus.ProgOlymp.Api.Domain;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

public interface IContestService
{
    Task<int> AddContestAsync(string name,
                              string title,
                              DateTimeOffset registrationStart,
                              DateTimeOffset registrationFinish,
                              long? preliminaryStageId,
                              long? finalStageId);

    Task<Contest?> GetContestAsync(string contestName);

    Task<bool> IsContestExistAsync(string contestName);
}
