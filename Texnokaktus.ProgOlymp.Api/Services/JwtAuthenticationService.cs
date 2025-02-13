using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Texnokaktus.ProgOlymp.Api.Extensions;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;
using Texnokaktus.ProgOlymp.Api.Models;
using Texnokaktus.ProgOlymp.Api.Settings;

namespace Texnokaktus.ProgOlymp.Api.Services;

using IAuthenticationService = Abstractions.IAuthenticationService;

public class JwtAuthenticationService(IUserService userService,
                                      IOptions<JwtSettings> jwtSettings,
                                      TimeProvider timeProvider) : IAuthenticationService
{
    public async Task<Ok<TokenModel>> AuthenticateUserAsync(HttpContext context, string code)
    {
        var user = await userService.AuthenticateUserAsync(code);
        
        var claims = new Claim[]
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Login)
        };

        return TypedResults.Ok(GetTokenModel(claims));
    }

    private TokenModel GetTokenModel(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Convert.FromBase64String(jwtSettings.Value.IssuerSigningKey));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var now = timeProvider.GetUtcNow();
        var expiration = now.Add(jwtSettings.Value.Validity);

        var token = new JwtSecurityToken(jwtSettings.Value.ClaimsIssuer,
                                         jwtSettings.Value.Audience,
                                         claims,
                                         now.UtcDateTime,
                                         expiration.UtcDateTime,
                                         signingCredentials);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new(tokenString, expiration);
    }
}
