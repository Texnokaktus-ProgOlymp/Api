using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.Results;

namespace Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;

public interface IResultServiceClient
{
    Task<Contest> GetContestAsync(string contestName, ContestStage contestStage, CancellationToken cancellationToken);
    Task<ContestResults> GetResultsAsync(string contestName, ContestStage contestStage, CancellationToken cancellationToken);
    Task<ParticipantResults> GetParticipantResultsAsync(string contestName, ContestStage contestStage, int participantId, CancellationToken cancellationToken);

    Task AddContestAsync(string contestName, ContestStage contestStage, long stageId, CancellationToken cancellationToken);
    Task AddProblemAsync(string contestName, ContestStage contestStage, string alias, string name, CancellationToken cancellationToken);
    Task AddResultAsync(string contestName, ContestStage contestStage, string alias, int participantId, decimal baseScore, CancellationToken cancellationToken);
    Task AddResultAdjustmentAsync(string contestName, ContestStage contestStage, string alias, int participantId, decimal adjustment, string? comment, CancellationToken cancellationToken);
}
