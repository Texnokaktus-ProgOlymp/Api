using Texnokaktus.ProgOlymp.Api.Domain;

namespace Texnokaktus.ProgOlymp.Api.Models;

public record ContestRegistrationState(int ContestId,
                                       string ContestName,
                                       DateTimeOffset RegistrationStart,
                                       DateTimeOffset RegistrationFinish,
                                       RegistrationState State);
