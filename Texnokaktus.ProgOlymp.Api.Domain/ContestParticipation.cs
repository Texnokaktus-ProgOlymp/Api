namespace Texnokaktus.ProgOlymp.Api.Domain;

public record ContestParticipation
{
    public required bool IsUserRegistered { get; init; }
    public ContestStageParticipation? PreliminaryStageParticipation { get; init; }
    public ContestStageParticipation? FinalStageParticipation { get; init; }
}
