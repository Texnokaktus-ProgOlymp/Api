namespace Texnokaktus.ProgOlymp.Api.Domain;

public record ContestStageParticipation(long? ContestId,
                                        DateTimeOffset? Start,
                                        DateTimeOffset? Finish,
                                        ParticipationState State);
 
