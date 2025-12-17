namespace Texnokaktus.ProgOlymp.Api.Models;

public record ContestParticipation
{
    public required bool IsUserRegistered { get; init; }
    public ContestStageParticipation? PreliminaryStageParticipation { get; init; }
    public ContestStageParticipation? FinalStageParticipation { get; init; }
}
