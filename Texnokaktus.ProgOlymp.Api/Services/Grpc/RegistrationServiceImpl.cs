using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.Registration;

namespace Texnokaktus.ProgOlymp.Api.Services.Grpc;

public class RegistrationServiceImpl(IRegistrationService registrationService) : Common.Contracts.Grpc.Registration.RegistrationService.RegistrationServiceBase
{
    public override async Task<QualifyParticipantsResponse> QualifyParticipants(QualifyParticipantsRequest request, ServerCallContext context)
    {
        var pairs = await registrationService.QualifyUsersAsync(request.ContestName, request.ScoreThreshold, context.CancellationToken);
        return new()
        {
            Result =
            {
                pairs.Select(pair => new QualifiedParticipant
                {
                    ParticipantId = pair.Key.Id,
                    ParticipantUid = pair.Key.Uid.ToString(),
                    Login = pair.Value.Login,
                    Password = pair.Value.Password
                })
            }
        };
    }

    public override async Task<QualifyParticipantsResponse> QualifyParticipantsDryRun(QualifyParticipantsRequest request, ServerCallContext context)
    {
        var pairs = await registrationService.QualifyUsersDryRunAsync(request.ContestName, request.ScoreThreshold, context.CancellationToken);
        return new()
        {
            Result =
            {
                pairs.Select(pair => new QualifiedParticipant
                {
                    ParticipantId = pair.Key.Id,
                    ParticipantUid = pair.Key.Uid.ToString(),
                    Login = pair.Value.Login,
                    Password = pair.Value.Password
                })
            }
        };
    }
}
