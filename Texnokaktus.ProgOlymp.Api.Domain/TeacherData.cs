namespace Texnokaktus.ProgOlymp.Api.Domain;

public record TeacherData
{
    public Name Name { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string School { get; init; }
}
