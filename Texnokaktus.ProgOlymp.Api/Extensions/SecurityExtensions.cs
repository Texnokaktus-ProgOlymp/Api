using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace Texnokaktus.ProgOlymp.Api.Extensions;

internal static class SecurityExtensions
{
    public static AuthenticationBuilder AddConfiguredJwtBearer(this AuthenticationBuilder builder,
                                                               IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
                       ?? throw new("No JwtSettings in the configuration");

        return builder.AddJwtBearer(options =>
        {
            options.ClaimsIssuer = jwtSettings.ClaimsIssuer;
            options.Audience = jwtSettings.Audience;
            options.TokenValidationParameters = new()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtSettings.IssuerSigningKey))
            };
        });
    }
}

public class JwtSettings
{
    public string ClaimsIssuer { get; init; }
    public string Audience { get; init; }
    public string IssuerSigningKey { get; init; }
    public TimeSpan Validity { get; init; }
}
