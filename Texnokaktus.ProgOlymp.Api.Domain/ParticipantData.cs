namespace Texnokaktus.ProgOlymp.Api.Domain;

public record ParticipantData
{
    public Name Name { get; init; }
    public DateOnly BirthDate { get; init; }
    public string? Snils { get; init; }
    public bool IsSnilsValid { get; init; }
    public string Email { get; init; }
    public string School { get; init; }
    public string Region { get; init; }
    public int Grade { get; init; }
}
