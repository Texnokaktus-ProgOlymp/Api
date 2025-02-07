namespace Texnokaktus.ProgOlymp.Api.DataAccess.Entities;

public class ThirdPerson
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string? Patronym { get; init; }
    public required string? Email { get; init; }
    public required string? Phone { get; init; }
}
