namespace Texnokaktus.ProgOlymp.Api.Domain;

public record ContestApplications
{
    public required Contest Contest { get; init; }
    public required IEnumerable<Application> Applications { get; init; }
}
