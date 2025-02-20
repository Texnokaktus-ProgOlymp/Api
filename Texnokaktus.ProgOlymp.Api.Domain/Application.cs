namespace Texnokaktus.ProgOlymp.Api.Domain;

public record Application(int Id,
                          User User,
                          DateTimeOffset Created,
                          ParticipantData ParticipantData,
                          ParentData ParentData,
                          TeacherData TeacherData,
                          bool PersonalDataConsent);
