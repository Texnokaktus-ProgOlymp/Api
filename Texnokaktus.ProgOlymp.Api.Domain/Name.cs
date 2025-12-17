namespace Texnokaktus.ProgOlymp.Api.Domain;

public record Name
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string? Patronym { get; init; }
}
