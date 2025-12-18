using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;

namespace Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;

public interface IContestDataServiceClient
{
    Task<string?> GetContestUrlAsync(long contestId);
    Task<ContestDescription> GetContestAsync(long contestId, CancellationToken cancellationToken);
    Task<IEnumerable<ContestProblem>> GetContestProblemsAsync(long contestId);
    Task<ContestStandings> GetStandingsAsync(long contestStageId, int pageIndex, int pageSize, string? participantSearch);
}
