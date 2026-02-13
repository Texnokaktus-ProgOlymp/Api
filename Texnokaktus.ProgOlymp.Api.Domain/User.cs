namespace Texnokaktus.ProgOlymp.Api.Domain;

public record User
{
    public int Id { get; init; }
    public string Login { get; init; }
    public string DisplayName { get; init; }
    public string? DefaultAvatar { get; init; }
}
