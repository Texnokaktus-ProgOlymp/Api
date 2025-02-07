using Texnokaktus.ProgOlymp.Api.DataAccess.Services.Abstractions;
using Texnokaktus.ProgOlymp.Api.Logic.Models;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services;

public class UserService(IUnitOfWork unitOfWork, IRegistrationService registrationService) : IUserService
{
    public Task<bool> IsUserRegisteredAsync(int contestId, string login) =>
        unitOfWork.ApplicationRepository.ExistsAsync(application => application.ContestId == contestId
                                                                 && application.YandexIdLogin == login);

    public async Task<int> RegisterUserAsync(UserInsertModel userInsertModel)
    {
        var entity = unitOfWork.ApplicationRepository.Add(userInsertModel.MapUserInsertModel());

        if (await unitOfWork.SaveChangesAsync() > 0)
            await registrationService.RegisterUserToPreliminaryStageAsync(userInsertModel.ContestId,
                                                                          userInsertModel.YandexIdLogin,
                                                                          userInsertModel.YandexIdLogin);

        return entity.Id;
    }
}

file static class MappingExtensions
{
    public static DataAccess.Models.UserInsertModel MapUserInsertModel(this UserInsertModel userInsertModel) =>
        new(userInsertModel.ContestId,
            userInsertModel.YandexIdLogin,
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
    
    private static DataAccess.Models.Name MapName(this Name name) =>
        new(name.FirstName,
            name.LastName,
            name.Patronym);
}
