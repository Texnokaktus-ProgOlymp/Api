using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;

namespace Texnokaktus.ProgOlymp.Api.Infrastructure.Clients;

public class RegistrationServiceClient(RegistrationService.RegistrationServiceClient client) : IRegistrationServiceClient
{
    public async Task RegisterParticipantAsync(long contestStageId, string login, string? displayName)
    {
        var request = new RegisterParticipantRequest
        {
            ContestStageId = contestStageId,
            YandexIdLogin = login,
            DisplayName = displayName
        };
        await client.RegisterParticipantAsync(request);
    }
}
