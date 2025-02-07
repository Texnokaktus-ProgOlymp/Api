namespace Texnokaktus.ProgOlymp.Api.Logic.Models;

public record Teacher(Name Name, string? Email, string? Phone, string School) : ThirdPerson(Name, Email, Phone);
