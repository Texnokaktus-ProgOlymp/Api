using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.Data;

namespace Texnokaktus.ProgOlymp.Api.Services.Grpc;

public class RegistrationDataServiceImpl(IRegistrationService registrationService) : RegistrationDataService.RegistrationDataServiceBase
{
    public override async Task<GetRegistrationsResponse> GetRegistrations(GetRegistrationsRequest request, ServerCallContext context) =>
        await registrationService.GetContestApplicationsAsync(request.ContestName) is { } contestApplications
            ? new GetRegistrationsResponse
            {
                Result = new()
                {
                    Contest = new()
                    {
                        Name = contestApplications.Contest.Name
                    },
                    Registrations =
                    {
                        contestApplications.Applications.Select(application => application.MapRegistration())
                    }
                }
            }
            : throw new RpcException(new(StatusCode.NotFound, $"Contest {request.ContestName} was not found"));
}

file static class MappingExtensions
{
    public static Registration MapRegistration(this Domain.Application application) =>
        new()
        {
            Id = application.Id,
            User = application.User.MapUser(),
            Created = application.Created.ToTimestamp(),
            ParticipantData = application.ParticipantData.MapParticipantData(),
            ParentData = application.ParentData.MapParentData(),
            TeacherData = application.TeacherData.MapTeacherData(),
            PersonalDataConsent = application.PersonalDataConsent,
            Uid = ByteString.CopyFrom(application.Uid?.ToByteArray() ?? [])
        };

    private static ParticipantData MapParticipantData(this Domain.ParticipantData participantData) =>
        new()
        {
            Name = participantData.Name.MapName(),
            BirthDate = DateTime.SpecifyKind(participantData.BirthDate.ToDateTime(new(0, 0)), DateTimeKind.Utc).ToTimestamp(),
            Snils = participantData.Snils,
            IsSnilsValid = participantData.IsSnilsValid,
            Email = participantData.Email,
            School = participantData.School,
            Region = participantData.Region,
            Grade = participantData.Grade
        };

    private static ParentData MapParentData(this Domain.ParentData parentData) =>
        new()
        {
            Name = parentData.Name.MapName(),
            Email = parentData.Email,
            Phone = parentData.Phone
        };

    private static TeacherData MapTeacherData(this Domain.TeacherData teacherData) =>
        new()
        {
            Name = teacherData.Name.MapName(),
            Email = teacherData.Email,
            Phone = teacherData.Phone,
            School = teacherData.School
        };

    private static User MapUser(this Domain.User user) =>
        new()
        {
            Id = user.Id,
            Login = user.Login,
            DisplayName = user.DisplayName,
            AvatarId = user.DefaultAvatar
        };

    private static Name MapName(this Domain.Name name) =>
        new()
        {
            FirstName = name.FirstName,
            LastName = name.LastName,
            Patronym = name.Patronym
        };
}
