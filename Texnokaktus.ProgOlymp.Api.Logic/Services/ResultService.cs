using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services;

public class ResultService(IContestDataServiceClient contestDataServiceClient) : IResultService
{
    public async Task<ContestResults> GetContestResultsAsync(string login, long contestStageId)
    {
        var standings = await contestDataServiceClient.GetStandingsAsync(contestStageId, 1, 1, login);

        var problemResults = standings.Titles
                                      .Zip(standings.Rows.First().ProblemResults)
                                      .Select(tuple => tuple.MapProblemResult())
                                      .ToArray();

        return new(false, problemResults);
    }
}

file static class MappingExtensions
{
    public static Domain.ProblemResult MapProblemResult(this (ContestStandingsTitle, Common.Contracts.Grpc.YandexContest.ProblemResult) tuple) =>
        new(tuple.Item1.Title,
            tuple.Item1.Name,
            tuple.Item2.Score,
            null);
}
