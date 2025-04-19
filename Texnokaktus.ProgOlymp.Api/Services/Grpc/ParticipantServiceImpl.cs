using Grpc.Core;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.Participants;

namespace Texnokaktus.ProgOlymp.Api.Services.Grpc;

public class ParticipantServiceImpl(IRegistrationService registrationService) : ParticipantService.ParticipantServiceBase
{
    public override async Task<GetContestParticipantsResponse> GetContestParticipants(GetContestParticipantsRequest request, ServerCallContext context)
    {
        var contestApplications = await registrationService.GetContestApplicationsAsync(request.ContestId)
                               ?? throw new RpcException(new(StatusCode.NotFound, $"Contest with Id {request.ContestId} was not found"));

        return new()
        {
            ContestName = contestApplications.Contest.Name,
            ParticipantGroups =
            {
                contestApplications.Applications
                                   .GroupBy(application => GetGroupName(application.ParticipantData.Grade),
                                            (groupName, applications) => new ParticipantGroup
                                            {
                                                Name = groupName,
                                                Participants =
                                                {
                                                    applications.Select(application => application.MapParticipant())
                                                }
                                            })
            }
        };
    }

    private static string GetGroupName(int grade) =>
        grade switch
        {
            8 or 9   => "8 - 9 класс",
            10 or 11 => "10 - 11 класс",
            _        => throw new ArgumentOutOfRangeException(nameof(grade), grade, "Invalid grade")
        };
}

file static class MappingExtensions
{
    public static Participant MapParticipant(this Domain.Application application) =>
        new()
        {
            Id = application.Id,
            Name = application.ParticipantData.Name.MapName(),
            Grade = application.ParticipantData.Grade
        };

    private static Name MapName(this Domain.Name name) =>
        new()
        {
            FirstName = name.FirstName,
            LastName = name.LastName,
            Patronym = name.Patronym
        };
}
