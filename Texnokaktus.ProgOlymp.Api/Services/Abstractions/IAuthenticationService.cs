using Microsoft.AspNetCore.Http.HttpResults;
using Texnokaktus.ProgOlymp.Api.Models;

namespace Texnokaktus.ProgOlymp.Api.Services.Abstractions;

public interface IAuthenticationService
{
    Task<Ok<TokenModel>> AuthenticateUserAsync(HttpContext context, string code);
}
