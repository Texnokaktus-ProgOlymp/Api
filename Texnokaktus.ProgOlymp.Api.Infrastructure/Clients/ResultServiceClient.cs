using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.Results;

namespace Texnokaktus.ProgOlymp.Api.Infrastructure.Clients;

public class ResultServiceClient(ResultService.ResultServiceClient client) : IResultServiceClient
{
    public async Task<Contest> GetContestAsync(string contestName, ContestStage contestStage, CancellationToken cancellationToken)
    {
        var request = new GetContestRequest
        {
            ContestName = contestName,
            Stage = contestStage
        };

        return await client.GetContestAsync(request, cancellationToken: cancellationToken);
    }

    public async Task<ContestResults> GetResultsAsync(string contestName, ContestStage contestStage, CancellationToken cancellationToken)
    {
        var request = new GetResultsRequest
        {
            ContestName = contestName,
            Stage = contestStage
        };

        return await client.GetResultsAsync(request, cancellationToken: cancellationToken);
    }

    public async Task<ParticipantResults> GetParticipantResultsAsync(string contestName, ContestStage contestStage, int participantId, CancellationToken cancellationToken)
    {
        var request = new GetResultsByParticipantRequest
        {
            ContestName = contestName,
            Stage = contestStage,
            ParticipantId = participantId
        };

        return await client.GetResultsByParticipantAsync(request, cancellationToken: cancellationToken);
    }

    public async Task AddContestAsync(string contestName, ContestStage contestStage, long stageId, CancellationToken cancellationToken)
    {
        var request = new AddContestRequest
        {
            ContestName = contestName,
            Stage = contestStage,
            StageId = stageId
        };

        await client.AddContestAsync(request, cancellationToken: cancellationToken);
    }

    public async Task AddProblemAsync(string contestName, ContestStage contestStage, string alias, string name, CancellationToken cancellationToken)
    {
        var request = new AddProblemRequest
        {
            ContestName = contestName,
            Stage = contestStage,
            Alias = alias,
            Name = name
        };

        await client.AddProblemAsync(request, cancellationToken: cancellationToken);
    }

    public async Task AddResultAsync(string contestName, ContestStage contestStage, string alias, int participantId, decimal baseScore, CancellationToken cancellationToken)
    {
        var request = new AddResultRequest
        {
            ContestName = contestName,
            Stage = contestStage,
            Alias = alias,
            ParticipantId = participantId,
            BaseScore = baseScore
        };

        await client.AddResultAsync(request, cancellationToken: cancellationToken);
    }

    public async Task AddResultAdjustmentAsync(string contestName, ContestStage contestStage, string alias, int participantId, decimal adjustment, string? comment, CancellationToken cancellationToken)
    {
        var request = new AddResultAdjustmentRequest
        {
            ContestName = contestName,
            Stage = contestStage,
            Alias = alias,
            ParticipantId = participantId,
            Adjustment = adjustment,
            Comment = comment
        };

        await client.AddResultAdjustmentAsync(request, cancellationToken: cancellationToken);
    }
}
