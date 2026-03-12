namespace Texnokaktus.ProgOlymp.Api.Domain;

public record Participation
{
    public long YandexContestId { get; init; }
    public ParticipationState State { get; init; }
    public DateTimeOffset? Start { get; init; }
    public DateTimeOffset? Finish { get; init; }
}
