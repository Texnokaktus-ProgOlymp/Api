namespace Texnokaktus.ProgOlymp.Api.Settings;

public class JwtSettings
{
    public string ClaimsIssuer { get; init; }
    public string Audience { get; init; }
    public string IssuerSigningKey { get; init; }
    public TimeSpan Validity { get; init; }
}
