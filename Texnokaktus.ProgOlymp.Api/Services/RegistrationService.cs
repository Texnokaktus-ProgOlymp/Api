using Microsoft.AspNetCore.Http.HttpResults;
using Texnokaktus.ProgOlymp.Api.Logic.Exceptions;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;
using Texnokaktus.ProgOlymp.Api.Models;
using Name = Texnokaktus.ProgOlymp.Api.Models.Name;

namespace Texnokaktus.ProgOlymp.Api.Services;

public class RegistrationService(IRegistrationService registrationService, ILogger<RegistrationService> logger) : Abstractions.IRegistrationService
{
    public async Task<Results<Created, NotFound, Conflict>> RegisterUserAsync(string contestName, int userId, ApplicationInsertModel insertModel)
    {
        if (await registrationService.IsUserRegisteredAsync(contestName, userId))
            return TypedResults.Conflict();

        try
        {
            var model = insertModel.MapUserInsertModel(contestName, userId);
            await registrationService.RegisterUserAsync(model);

            return TypedResults.Created();
        }
        catch (RegistrationClosedException e)
        {
            logger.LogWarning(e, "User {UserId} tried to register when registration is closed", userId);
            return TypedResults.Conflict();
        }
        catch (AlreadyRegisteredException e)
        {
            logger.LogWarning(e, "User {UserId} tried to register repeatedly", userId);
            return TypedResults.Conflict();
        }
    }
}

file static class MappingExtensions
{
    public static Logic.Models.ApplicationInsertModel MapUserInsertModel(this ApplicationInsertModel userInsertModel,
                                                                         string contestName,
                                                                         int userId) =>
        new(userId,
            contestName,
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
