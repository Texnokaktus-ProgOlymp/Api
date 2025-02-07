namespace Texnokaktus.ProgOlymp.Api.DataAccess.Models;

public record UserInsertModel(int ContestId,
                              string YandexIdLogin,
                              Name Name,
                              DateOnly BirthDate,
                              string Snils,
                              string Email,
                              string SchoolName,
                              int RegionId,
                              ThirdPerson Parent,
                              Teacher Teacher,
                              bool PersonalDataConsent,
                              int Grade);
