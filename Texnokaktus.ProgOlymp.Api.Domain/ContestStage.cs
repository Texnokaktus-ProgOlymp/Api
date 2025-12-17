namespace Texnokaktus.ProgOlymp.Api.Domain;

public record ContestStage
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required DateTimeOffset ContestStart { get; init; }
    public required DateTimeOffset? ContestFinish { get; init; }
    public required TimeSpan Duration { get; init; }
}
