namespace Texnokaktus.ProgOlymp.Api.Models;

public record TokenModel(string Token, DateTimeOffset Expiration, UserModel User);
