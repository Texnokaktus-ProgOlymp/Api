namespace Texnokaktus.ProgOlymp.Api.Domain;

public record ContestRegistrationState(int ContestId,
                                       string ContestName,
                                       DateTimeOffset RegistrationStart,
                                       DateTimeOffset RegistrationFinish,
                                       RegistrationState State);
