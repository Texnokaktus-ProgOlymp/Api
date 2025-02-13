namespace Texnokaktus.ProgOlymp.Api.Domain;

public record ContestParticipation(bool IsUserRegistered,
                                   ContestStageParticipation? PreliminaryStageParticipation,
                                   ContestStageParticipation? FinalStageParticipation);
