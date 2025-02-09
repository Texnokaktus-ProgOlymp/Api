namespace Texnokaktus.ProgOlymp.Api.DataAccess.Entities;

public class User
{
    public int Id { get; init; }
    public string Login { get; init; }
    public string DisplayName { get; set; }
    public string? DefaultAvatar { get; set; }
}
