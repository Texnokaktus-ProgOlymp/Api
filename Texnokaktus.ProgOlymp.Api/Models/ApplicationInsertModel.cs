namespace Texnokaktus.ProgOlymp.Api.Models;

public record ApplicationInsertModel(Name Name,
                                     DateOnly BirthDate,
                                     string Snils,
                                     string Email,
                                     string SchoolName,
                                     int RegionId,
                                     ThirdPerson Parent,
                                     Teacher Teacher,
                                     bool PersonalDataConsent,
                                     int Grade);
