using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services;

public class ResultService(IContestDataServiceClient contestDataServiceClient) : IResultService
{
    public async Task<ContestResults> GetContestResultsAsync(string login, long contestStageId)
    {
        var problems = await contestDataServiceClient.GetContestProblemsAsync(contestStageId);
        var stats = await contestDataServiceClient.GetParticipantStatsAsync(contestStageId, login);

        var problemResults = problems.GroupJoin(stats.Runs,
                                                problem => problem.Id,
                                                run => run.ProblemId,
                                                (problem, runs) => new ProblemResult(problem.Alias,
                                                                                     problem.Name,
                                                                                     runs.Max(run => run.Score),
                                                                                     null))
                                     .OrderBy(result => result.ProblemId)
                                     .ToArray();

        return new(false, problemResults);
    }
}
