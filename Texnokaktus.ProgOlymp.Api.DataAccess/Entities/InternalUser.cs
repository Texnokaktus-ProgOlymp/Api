namespace Texnokaktus.ProgOlymp.Api.DataAccess.Entities;

public class InternalUser
{
    public int Id { get; init; }
    public string Login { get; init; }
    public string Password { get; set; }
    public bool IsDeprecated { get; set; }
}
