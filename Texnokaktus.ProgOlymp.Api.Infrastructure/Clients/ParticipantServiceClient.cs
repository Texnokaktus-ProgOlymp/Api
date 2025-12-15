using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;

namespace Texnokaktus.ProgOlymp.Api.Infrastructure.Clients;

public class ParticipantServiceClient(ParticipantService.ParticipantServiceClient client) : IParticipantServiceClient
{
    public async Task<IEnumerable<ParticipantInfo>> GetContestParticipantsAsync(long contestStageId, CancellationToken cancellationToken)
    {
        var request = new ContestParticipantsRequest
        {
            ContestId = contestStageId
        };

        var response = await client.GetContestParticipantsAsync(request, cancellationToken: cancellationToken);
        return response.Result;
    }

    public async Task<ParticipantStatus> GetParticipantStatusAsync(long contestStageId, int participantId, CancellationToken cancellationToken)
    {
        var request = new ParticipantStatusRequest
        {
            ContestId = contestStageId,
            ParticipantId = participantId
        };

        var response = await client.GetParticipantStatusAsync(request, cancellationToken: cancellationToken);
        return response.Result;
    }

    public async Task<ParticipantStats> GetParticipantStatsAsync(long contestStageId, int participantId)
    {
        var request = new ParticipantStatsRequest
        {
            ContestId = contestStageId,
            ParticipantId = participantId
        };

        var response = await client.GetParticipantStatsAsync(request);
        return response.Result;
    }
}
