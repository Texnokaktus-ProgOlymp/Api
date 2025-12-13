using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.Api.DataAccess.Context;
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
                                 IUserService userService,
                                 AppDbContext context) : IRegistrationService
{
    private readonly Counter<int> _registeredUsers = MeterProvider.Meter.CreateRegisteredUsersCounter();
    private readonly Counter<int> _yandexContestRegisteredUsers = MeterProvider.Meter.CreateCounter<int>("users.registered.yandex-contest");

    public async Task<ContestRegistrationState?> GetRegistrationStateAsync(string contestName)
    {
        if (await contestService.GetContestAsync(contestName) is not { } contest) return null;

        return new(contest.Id,
                   contest.Name,
                   contest.RegistrationStart,
                   contest.RegistrationFinish,
                   GetState(contest, timeProvider.GetUtcNow()));
    }

    public Task<bool> IsUserRegisteredAsync(string contestName, int userId) =>
        context.Applications.AnyAsync(application => application.Contest.Name == contestName
                                                  && application.UserId == userId);

    public async Task<int> RegisterUserAsync(ApplicationInsertModel userInsertModel)
    {
        var contest = await contestService.GetRequiredContestAsync(userInsertModel.ContestName);
        var user = await userService.GetRequiredUserAsync(userInsertModel.UserId);

        var uid = Guid.NewGuid();

        var entity = context.Applications
                            .Add(new()
                             {
                                 UserId = userInsertModel.UserId,
                                 ContestId = contest.Id,
                                 Uid = uid,
                                 Created = timeProvider.GetUtcNow(),
                                 FirstName = userInsertModel.Name.FirstName,
                                 LastName = userInsertModel.Name.LastName,
                                 Patronym = userInsertModel.Name.Patronym,
                                 BirthDate = userInsertModel.BirthDate,
                                 Snils = userInsertModel.Snils,
                                 Email = userInsertModel.Email,
                                 SchoolName = userInsertModel.SchoolName,
                                 RegionId = userInsertModel.RegionId,
                                 Parent = new()
                                 {
                                     FirstName = userInsertModel.Parent.Name.FirstName,
                                     LastName = userInsertModel.Parent.Name.LastName,
                                     Patronym = userInsertModel.Parent.Name.Patronym,
                                     Email = userInsertModel.Parent.Email,
                                     Phone = userInsertModel.Parent.Phone
                                 },
                                 Teacher = new()
                                 {
                                     School = userInsertModel.Teacher.School,
                                     FirstName = userInsertModel.Teacher.Name.FirstName,
                                     LastName = userInsertModel.Teacher.Name.LastName,
                                     Patronym = userInsertModel.Teacher.Name.Patronym,
                                     Email = userInsertModel.Teacher.Email,
                                     Phone = userInsertModel.Teacher.Phone
                                 },
                                 PersonalDataConsent = userInsertModel.PersonalDataConsent,
                                 Grade = userInsertModel.Grade
                             })
                            .Entity;

        await context.SaveChangesAsync();

        _registeredUsers.Add(1,
                             KeyValuePair.Create<string, object?>("contestName", userInsertModel.ContestName),
                             KeyValuePair.Create<string, object?>("regionId", userInsertModel.RegionId));

        await RegisterUserToPreliminaryStageAsync(contest, user.Login, uid.ToString("N"));

        return entity.Id;
    }

    public async Task<ContestApplications?> GetContestApplicationsAsync(string contestName)
    {
        if (await contestService.GetContestAsync(contestName) is not { } contest) return null;

        var applications = await context.Applications
                                        .Include(application => application.User)
                                        .Include(application => application.Region)
                                        .Where(application => application.Contest.Name == contestName)
                                        .Select(application => application.MapDomainApplication())
                                        .ToArrayAsync();

        return new(contest, applications);
    }

    private async Task RegisterUserToPreliminaryStageAsync(Contest contest, string login, string? displayName)
    {
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
