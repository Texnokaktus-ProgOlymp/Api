using Texnokaktus.ProgOlymp.Api.Domain;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

public interface IResultService
{
    Task<ContestResults> GetContestResultsAsync(long contestStageId, int participantId);
}
