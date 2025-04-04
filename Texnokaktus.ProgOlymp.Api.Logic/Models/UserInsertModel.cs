namespace Texnokaktus.ProgOlymp.Api.Logic.Models;

public record ApplicationInsertModel(int UserId,
                                     int ContestId,
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
