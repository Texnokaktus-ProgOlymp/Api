using Texnokaktus.ProgOlymp.Api.DataAccess.Entities;

namespace Texnokaktus.ProgOlymp.Api.DataAccess.Models;

public record ApplicationInsertModel(int UserId,
                                     int ContestId,
                                     DateTimeOffset Created,
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
