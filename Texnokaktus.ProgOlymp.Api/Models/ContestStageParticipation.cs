using Texnokaktus.ProgOlymp.Api.Domain;

namespace Texnokaktus.ProgOlymp.Api.Models;

public record ContestStageParticipation
{
    public long? ContestId { get; init; }
    public DateTimeOffset? Start { get; init; }
    public DateTimeOffset? Finish { get; init; }
    public ParticipationState State { get; init; }
}
