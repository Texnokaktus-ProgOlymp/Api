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
        var contest = await contestService.GetContestAsync(userInsertModel.ContestName)
                   ?? throw new ContestNotFoundException(userInsertModel.ContestName);

        var userLogin = await context.Users
                                     .Where(user => user.Id == userInsertModel.UserId)
                                     .Select(user => user.Login)
                                     .FirstOrDefaultAsync()
                     ?? throw new UserNotFoundException(userInsertModel.UserId);

        var uid = Guid.NewGuid();

        await using var transaction = await context.Database.BeginTransactionAsync();

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

        entity.PreliminaryStageParticipation = new()
        {
            ContestUserId = await RegisterUserToPreliminaryStageAsync(contest, userLogin, uid.ToString("N")),
            State = DataAccess.Entities.ParticipationState.NotStarted
        };

        await context.SaveChangesAsync();

        await transaction.CommitAsync();

        _registeredUsers.Add(1,
                             KeyValuePair.Create<string, object?>("contestName", userInsertModel.ContestName),
                             KeyValuePair.Create<string, object?>("regionId", userInsertModel.RegionId));

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

    private async Task<long> RegisterUserToPreliminaryStageAsync(Contest contest, string login, string? displayName)
    {
        var contestUserId = await registrationServiceClient.RegisterParticipantAsync(contest.PreliminaryStage!.Id, login, displayName);
        _yandexContestRegisteredUsers.Add(1, KeyValuePair.Create<string, object?>("contestStageId", contest.PreliminaryStage.Id));
        return contestUserId;
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
    public static Application MapDomainApplication(this DataAccess.Entities.Application application) =>
        new(application.Id,
            application.Uid,
            application.User.MapUser(),
            application.Created,
            application.MapParticipantData(),
            application.Parent.MapParentData(),
            application.Teacher.MapTeacherData(),
            application.PersonalDataConsent);

    private static ParticipantData MapParticipantData(this DataAccess.Entities.Application application) =>
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

    private static ParentData MapParentData(this DataAccess.Entities.ThirdPerson parent) =>
        new(new(parent.FirstName,
                parent.LastName,
                parent.Patronym),
            parent.Email,
            parent.Phone);

    private static TeacherData MapTeacherData(this DataAccess.Entities.Teacher teacher) =>
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
