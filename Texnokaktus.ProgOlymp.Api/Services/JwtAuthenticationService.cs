using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Texnokaktus.ProgOlymp.Api.Extensions;
using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Api.Models;

namespace Texnokaktus.ProgOlymp.Api.Services;

using IAuthenticationService = Abstractions.IAuthenticationService;

public class JwtAuthenticationService(IYandexIdUserServiceClient yandexIdUserServiceClient,
                                      IOptions<JwtSettings> jwtSettings,
                                      TimeProvider timeProvider) : IAuthenticationService
{
    public async Task<Ok<TokenModel>> AuthenticateUserAsync(HttpContext context, string code)
    {
        var user = await yandexIdUserServiceClient.AuthenticateUserAsync(code);
        
        var claims = new Claim[]
        {
            new(ClaimTypes.NameIdentifier, 0.ToString()),
            new(ClaimTypes.Name, user.Login)
        };

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

        return TypedResults.Ok(new TokenModel(tokenString,
                                              expiration,
                                              new(user.Login,
                                                  user.DisplayName,
                                                  user.Avatar?.AvatarId)));
    }
}
