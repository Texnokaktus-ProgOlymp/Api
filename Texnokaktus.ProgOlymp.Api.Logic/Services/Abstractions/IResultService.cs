using Texnokaktus.ProgOlymp.Api.Domain;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

public interface IResultService
{
    Task<ContestResults> GetContestResultsAsync(string login, long contestStageId);
}
