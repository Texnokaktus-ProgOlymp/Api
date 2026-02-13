namespace Texnokaktus.ProgOlymp.Api.Domain;

public record ParentData
{
    public Name Name { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
}
