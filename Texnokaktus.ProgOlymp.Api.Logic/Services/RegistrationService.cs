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

    public Task<bool> IsUserRegisteredAsync(string contestName, int userId) =>
        context.Applications.AnyAsync(application => application.Contest.Name == contestName
                                                  && application.UserId == userId);

    public async Task<int> RegisterUserAsync(ApplicationInsertModel userInsertModel)
    {
        var contest = await contestService.GetContestAsync(userInsertModel.ContestName)
                   ?? throw new ContestNotFoundException(userInsertModel.ContestName);

        if (contest.RegistrationState != RegistrationState.InProgress)
            throw new RegistrationClosedException(contest.Name);

        var userLogin = await context.Users
                                     .Where(user => user.Id == userInsertModel.UserId)
                                     .Select(user => user.Login)
                                     .FirstOrDefaultAsync()
                     ?? throw new UserNotFoundException(userInsertModel.UserId);

        if (await context.Applications.AnyAsync(application => application.UserId == userInsertModel.UserId
                                                            && application.ContestId == contest.Id))
            throw new AlreadyRegisteredException(contest.Name, userInsertModel.UserId);

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

    public async Task<ContestApplications?> GetContestApplicationsAsync(string contestName) =>
        await contestService.GetContestAsync(contestName) is { } contest
            ? new()
            {
                Contest = contest,
                Applications = await context.Applications
                                            .Include(application => application.User)
                                            .Include(application => application.Region)
                                            .Where(application => application.Contest.Name == contestName)
                                            .Select(application => application.MapDomainApplication())
                                            .ToArrayAsync()
            }
            : null;

    private async Task<long> RegisterUserToPreliminaryStageAsync(Contest contest, string login, string? displayName)
    {
        var contestUserId = await registrationServiceClient.RegisterParticipantAsync(contest.PreliminaryStage!.Id, login, displayName);
        _yandexContestRegisteredUsers.Add(1, KeyValuePair.Create<string, object?>("contestStageId", contest.PreliminaryStage.Id));
        return contestUserId;
    }
}

file static class MappingExtensions
{
    public static Application MapDomainApplication(this DataAccess.Entities.Application application) =>
        new()
        {
            Id = application.Id,
            Uid = application.Uid,
            User = application.User.MapUser(),
            Created = application.Created,
            ParticipantData = application.MapParticipantData(),
            ParentData = application.Parent.MapParentData(),
            TeacherData = application.Teacher.MapTeacherData(),
            PersonalDataConsent = application.PersonalDataConsent
        };

    private static ParticipantData MapParticipantData(this DataAccess.Entities.Application application) =>
        new()
        {
            Name = new()
            {
                FirstName = application.FirstName,
                LastName = application.LastName,
                Patronym = application.Patronym
            },
            BirthDate = application.BirthDate,
            Snils = application.Snils,
            IsSnilsValid = true, // TODO Add validation
            Email = application.Email,
            School = application.SchoolName,
            Region = application.Region.Name,
            Grade = application.Grade
        };

    private static ParentData MapParentData(this DataAccess.Entities.ThirdPerson parent) =>
        new()
        {
            Name = new()
            {
                FirstName = parent.FirstName,
                LastName = parent.LastName,
                Patronym = parent.Patronym
            },
            Email = parent.Email,
            Phone = parent.Phone
        };

    private static TeacherData MapTeacherData(this DataAccess.Entities.Teacher teacher) =>
        new()
        {
            Name = new()
            {
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Patronym = teacher.Patronym
            },
            Email = teacher.Email,
            Phone = teacher.Phone,
            School = teacher.School
        };

    private static User MapUser(this DataAccess.Entities.User user) =>
        new()
        {
            Id = user.Id,
            Login = user.Login,
            DisplayName = user.DisplayName,
            DefaultAvatar = user.DefaultAvatar
        };
}
