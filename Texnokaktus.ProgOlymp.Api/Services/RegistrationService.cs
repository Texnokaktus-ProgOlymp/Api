using Microsoft.AspNetCore.Http.HttpResults;
using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;
using Texnokaktus.ProgOlymp.Api.Models;
using ContestRegistrationState = Texnokaktus.ProgOlymp.Api.Models.ContestRegistrationState;
using Name = Texnokaktus.ProgOlymp.Api.Models.Name;

namespace Texnokaktus.ProgOlymp.Api.Services;

public class RegistrationService(IContestService contestService, IRegistrationService registrationService) : Abstractions.IRegistrationService
{
    public async Task<Results<Ok<ContestRegistrationState>, NotFound>> GetRegistrationStateAsync(int contestId)
    {
        return await registrationService.GetRegistrationStateAsync(contestId) is { } registrationState
                   ? TypedResults.Ok(new ContestRegistrationState(registrationState.ContestId,
                                                                  registrationState.ContestName,
                                                                  registrationState.RegistrationStart,
                                                                  registrationState.RegistrationFinish,
                                                                  registrationState.State))
                   : TypedResults.NotFound();
    }

    public async Task<Results<Created, NotFound, Conflict>> RegisterUserAsync(int contestId, int userId, ApplicationInsertModel insertModel)
    {
        if (await contestService.GetContestAsync(contestId) is null)
            return TypedResults.NotFound();

        if (await registrationService.GetRegistrationStateAsync(contestId) is { State: not RegistrationState.InProgress })
            return TypedResults.Conflict();

        if (await registrationService.IsUserRegisteredAsync(contestId, userId))
            return TypedResults.Conflict();

        var model = insertModel.MapUserInsertModel(contestId, userId);
        await registrationService.RegisterUserAsync(model);

        return TypedResults.Created();
    }
}

file static class MappingExtensions
{
    public static Logic.Models.ApplicationInsertModel MapUserInsertModel(this ApplicationInsertModel userInsertModel,
                                                                         int contestId,
                                                                         int userId) =>
        new(userId,
            contestId,
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
    
    private static Logic.Models.Teacher MapTeacher(this Teacher teacher)
    {
        var thirdPerson = teacher.MapThirdPerson();
        return new(thirdPerson.Name,
                   thirdPerson.Email,
                   thirdPerson.Phone,
                   teacher.School);
    }
    
    private static Logic.Models.ThirdPerson MapThirdPerson(this ThirdPerson thirdPerson) =>
        new(thirdPerson.Name.MapName(),
            thirdPerson.Email,
            thirdPerson.Phone);

    private static Logic.Models.Name MapName(this Name name) =>
        new(name.FirstName,
            name.LastName,
            name.Patronym);
}
