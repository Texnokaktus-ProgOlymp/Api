namespace Texnokaktus.ProgOlymp.Api.Domain;

public record ContestStageParticipation(DateTimeOffset Start,
                                        DateTimeOffset? Finish,
                                        ParticipationState State,
                                        // long? Url,
                                        ContestResults? Results);
