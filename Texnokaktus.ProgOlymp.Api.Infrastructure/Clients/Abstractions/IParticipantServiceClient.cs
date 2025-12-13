using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;

namespace Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;

public interface IParticipantServiceClient
{
    Task<IEnumerable<ParticipantInfo>> GetContestParticipantsAsync(long contestStageId);
    Task<ParticipantStatus> GetParticipantStatusAsync(long contestStageId, int participantId);
    Task<ParticipantStats> GetParticipantStatsAsync(long contestStageId, int participantId);
}
