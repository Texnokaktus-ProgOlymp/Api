using Texnokaktus.ProgOlymp.Api.Domain;

namespace Texnokaktus.ProgOlymp.Api.Models;

public record ContestRegistrationState(string Title,
                                       DateTimeOffset RegistrationStart,
                                       DateTimeOffset RegistrationFinish,
                                       RegistrationState State);
