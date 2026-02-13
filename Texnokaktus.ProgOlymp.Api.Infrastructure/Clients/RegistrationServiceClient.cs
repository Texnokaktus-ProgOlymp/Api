using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;

namespace Texnokaktus.ProgOlymp.Api.Infrastructure.Clients;

public class RegistrationServiceClient(RegistrationService.RegistrationServiceClient client) : IRegistrationServiceClient
{
    public async Task<long> RegisterParticipantAsync(long contestStageId, string login, string? displayName)
    {
        var request = new RegisterParticipantRequest
        {
            ContestStageId = contestStageId,
            YandexIdLogin = login,
            DisplayName = displayName
        };
        var response = await client.RegisterParticipantAsync(request);
        return response.ContestUserId;
    }
}
