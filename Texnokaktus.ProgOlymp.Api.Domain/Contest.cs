namespace Texnokaktus.ProgOlymp.Api.Domain;

public record Contest(int Id,
                      string Name, 
                      DateTimeOffset RegistrationStart,
                      DateTimeOffset RegistrationFinish,
                      ContestStage? PreliminaryStage,
                      ContestStage? FinalStage);
