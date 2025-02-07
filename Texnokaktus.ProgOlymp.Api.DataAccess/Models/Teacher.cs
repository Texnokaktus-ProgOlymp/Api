namespace Texnokaktus.ProgOlymp.Api.DataAccess.Models;

public record Teacher(Name Name, string? Email, string? Phone, string School) : ThirdPerson(Name, Email, Phone);
