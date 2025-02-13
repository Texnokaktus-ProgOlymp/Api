using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using Texnokaktus.ProgOlymp.Api.DataAccess.Repositories.Abstractions;
using Texnokaktus.ProgOlymp.Api.Settings;

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
            options.TokenValidationParameters = new()
            {
                ValidIssuer = jwtSettings.ClaimsIssuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtSettings.IssuerSigningKey))
            };

            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            options.Events ??= new();

            options.Events.OnTokenValidated = async context =>
            {
                if (!int.TryParse(context.Principal?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value, out var result))
                    context.Fail("Unable to get user id from token");

                var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

                if (!await userRepository.ExistsAsync(user => user.Id == result))
                    context.Fail("User does not exist");
            };
        });;
    }
}
