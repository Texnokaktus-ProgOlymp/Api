namespace Texnokaktus.ProgOlymp.Api.DataAccess.Entities;

public class User
{
    public int Id { get; init; }
    public required DateTimeOffset Created { get; init; }
    public required string Login { get; init; }
    public required string DisplayName { get; set; }
    public required string? DefaultAvatar { get; set; }
}
