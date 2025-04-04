using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using Texnokaktus.ProgOlymp.Api.DataAccess.Services.Abstractions;
using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Api.Logic.Exceptions;
using Texnokaktus.ProgOlymp.Api.Logic.Models;
using Texnokaktus.ProgOlymp.Api.Logic.Observability;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services;

public class RegistrationService(IContestService contestService,
                                 IRegistrationServiceClient registrationServiceClient,
                                 TimeProvider timeProvider,
                                 IUnitOfWork unitOfWork,
                                 IUserService userService) : IRegistrationService
{
    private readonly Counter<int> _registeredUsers = MeterProvider.Meter.CreateRegisteredUsersCounter();
    private readonly Counter<int> _yandexContestRegisteredUsers = MeterProvider.Meter.CreateCounter<int>("users.registered.yandex-contest");

    public async Task<ContestRegistrationState?> GetRegistrationStateAsync(int contestId)
    {
        if (await contestService.GetContestAsync(contestId) is not { } contest) return null;

        return new(contest.Id,
                   contest.Name,
                   contest.RegistrationStart,
                   contest.RegistrationFinish,
                   GetState(contest, timeProvider.GetUtcNow()));
    }

    public Task<bool> IsUserRegisteredAsync(int contestId, int userId) =>
        unitOfWork.ApplicationRepository.ExistsAsync(application => application.ContestId == contestId
                                                                 && application.UserId == userId);

    public async Task<int> RegisterUserAsync(ApplicationInsertModel userInsertModel)
    {
        var uid = Guid.NewGuid();

        var entity = unitOfWork.ApplicationRepository.Add(userInsertModel.MapUserInsertModel(timeProvider.GetUtcNow(), uid));

        await unitOfWork.SaveChangesAsync();

        _registeredUsers.Add(1,
                             KeyValuePair.Create<string, object?>("contestId", userInsertModel.ContestId),
                             KeyValuePair.Create<string, object?>("regionId", userInsertModel.RegionId));

        var user = await userService.GetByIdAsync(userInsertModel.UserId)
                ?? throw new UserNotFoundException(userInsertModel.UserId);

        await RegisterUserToPreliminaryStageAsync(userInsertModel.ContestId, user.Login, uid.ToString("N"));

        return entity.Id;
    }

    public async Task<ContestApplications?> GetContestApplicationsAsync(int contestId)
    {
        if (await contestService.GetContestAsync(contestId) is not { } contest) return null;

        var applications = await unitOfWork.ApplicationRepository.GetApplicationsAsync(contestId);

        return new(contest, applications.Select(application => application.MapDomainApplication()));
    }

    private async Task RegisterUserToPreliminaryStageAsync(int contestId, string login, string? displayName)
    {
        if (await contestService.GetContestAsync(contestId) is not { } contest)
            throw new ContestNotFoundException(contestId);

        await registrationServiceClient.RegisterParticipantAsync(contest.PreliminaryStage!.Id, login, displayName);
        _yandexContestRegisteredUsers.Add(1, KeyValuePair.Create<string, object?>("contestStageId", contest.PreliminaryStage.Id));
    }

    [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
    private static RegistrationState GetState(Contest contest, DateTimeOffset now)
    {
        if (contest.PreliminaryStage is null)
            return RegistrationState.Unavailable;

        if (now < contest.RegistrationStart) return RegistrationState.NotStarted;
        if (now >= contest.RegistrationFinish) return RegistrationState.Finished;
        return RegistrationState.InProgress;
    }
}

file static class MappingExtensions
{
    public static DataAccess.Models.ApplicationInsertModel MapUserInsertModel(this ApplicationInsertModel userInsertModel, DateTimeOffset created, Guid uid) =>
        new(userInsertModel.UserId,
            userInsertModel.ContestId,
            uid,
            created,
            userInsertModel.Name.MapName(),
            userInsertModel.BirthDate,
            userInsertModel.Snils,
            userInsertModel.Email,
            userInsertModel.SchoolName,
            userInsertModel.RegionId,
            userInsertModel.Parent.MapThirdPerson(),
            userInsertModel.Teacher.MapTeacher(),
            userInsertModel.PersonalDataConsent,
            userInsertModel.Grade);

    private static DataAccess.Models.Teacher MapTeacher(this Teacher teacher)
    {
        var thirdPerson = teacher.MapThirdPerson();
        return new(thirdPerson.Name,
                   thirdPerson.Email,
                   thirdPerson.Phone,
                   teacher.School);
    }

    private static DataAccess.Models.ThirdPerson MapThirdPerson(this Models.ThirdPerson thirdPerson) =>
        new(thirdPerson.Name.MapName(),
            thirdPerson.Email,
            thirdPerson.Phone);

    private static DataAccess.Models.Name MapName(this Models.Name name) =>
        new(name.FirstName,
            name.LastName,
            name.Patronym);

    public static Domain.Application MapDomainApplication(this DataAccess.Entities.Application application) =>
        new(application.Id,
            application.Uid,
            application.User.MapUser(),
            application.Created,
            application.MapParticipantData(),
            application.Parent.MapParentData(),
            application.Teacher.MapTeacherData(),
            application.PersonalDataConsent);

    private static Domain.ParticipantData MapParticipantData(this DataAccess.Entities.Application application) =>
        new(new(application.FirstName,
                application.LastName,
                application.Patronym),
            application.BirthDate,
            application.Snils,
            true, // TODO Add validation
            application.Email,
            application.SchoolName,
            application.Region.Name,
            application.Grade);

    private static Domain.ParentData MapParentData(this DataAccess.Entities.ThirdPerson parent) =>
        new(new(parent.FirstName,
                parent.LastName,
                parent.Patronym),
            parent.Email,
            parent.Phone);

    private static Domain.TeacherData MapTeacherData(this DataAccess.Entities.Teacher teacher) =>
        new(new(teacher.FirstName,
                teacher.LastName,
                teacher.Patronym),
            teacher.Email,
            teacher.Phone,
            teacher.School);

    private static User MapUser(this DataAccess.Entities.User user) =>
        new(user.Id,
            user.Login,
            user.DisplayName,
            user.DefaultAvatar);
}
