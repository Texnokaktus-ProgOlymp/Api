namespace Texnokaktus.ProgOlymp.Api.Domain;

public record Application(int Id,
                          Guid? Uid,
                          User User,
                          DateTimeOffset Created,
                          ParticipantData ParticipantData,
                          ParentData ParentData,
                          TeacherData TeacherData,
                          bool PersonalDataConsent);
