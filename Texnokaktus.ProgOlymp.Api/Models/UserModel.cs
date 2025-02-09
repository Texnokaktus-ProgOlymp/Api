namespace Texnokaktus.ProgOlymp.Api.Models;

public record UserModel(string Login, string DisplayName, string? AvatarId);

public record TokenModel(string Toke, DateTimeOffset Expiration, UserModel User);
