using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Api.Models;
using Texnokaktus.ProgOlymp.Api.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Endpoints;

internal static class EndpointsMapper
{
    public static IEndpointRouteBuilder MapContestEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("api/contests");

        group.MapGet("{contestId:int}", (int contestId, IRegistrationService registrationStateService) => registrationStateService.GetRegistrationStateAsync(contestId));

        group.MapPost("contests/{contestId:int}/register",
                      (int contestId,
                       ApplicationInsertModel model,
                       HttpContext context,
                       IRegistrationService service) =>
                      {
                          var login = int.Parse(context.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
                          return service.RegisterUserAsync(contestId, login, model);
                      })
             .RequireAuthorization();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapRegionEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/regions", (Logic.Services.Abstractions.IRegionService s) => s.GetAllRegionsAsync());

        return builder;
    }

    public static IEndpointRouteBuilder MapAuthorizationEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("authorize");

        group.MapGet("url",
                     async (string? redirectUrl,
                            [FromServices]IYandexIdUserServiceClient c) => TypedResults.Ok(await c.GetOAuthUrlAsync(redirectUrl)));

        group.MapGet("redirect",
                     async (string? redirectUrl,
                            [FromServices]IYandexIdUserServiceClient c) =>
                         TypedResults.Redirect(await c.GetOAuthUrlAsync(redirectUrl)));

        group.MapPost("",
                      (AuthorizationModel model,
                       HttpContext context,
                       IAuthenticationService service) => service.AuthenticateUserAsync(context, model.Code));

        return builder;
    }
}
