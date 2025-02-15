using Microsoft.AspNetCore.Mvc;
using Texnokaktus.ProgOlymp.Api.Extensions;
using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Api.Models;
using Texnokaktus.ProgOlymp.Api.Services.Abstractions;
using Texnokaktus.ProgOlymp.Api.Validators;

namespace Texnokaktus.ProgOlymp.Api.Endpoints;

internal static class EndpointsMapper
{
    public static IEndpointRouteBuilder MapContestEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("contests/{contestId:int}");

        group.MapGet("",
                     (int contestId, IRegistrationService registrationStateService) =>
                         registrationStateService.GetRegistrationStateAsync(contestId));

        group.MapPost("register",
                      (int contestId, ApplicationInsertModel model, HttpContext context, IRegistrationService service)
                          => service.RegisterUserAsync(contestId, context.GetUserId(), model))
             .AddEndpointFilter<ValidationFilter<ApplicationInsertModel>>()
             .RequireAuthorization();

        group.MapGet("participation",
                     (int contestId, IParticipationService participationService, HttpContext context)
                         => participationService.GetParticipationAsync(context.GetUserId(), contestId))
             .RequireAuthorization();

        return builder;
    }

    public static IEndpointRouteBuilder MapRegionEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("regions", (Logic.Services.Abstractions.IRegionService s) => s.GetAllRegionsAsync());
        
        /*
         * TODO Remove
         */

        // builder.MapGet("results",
        //                (string login, int contestId, Logic.Services.Abstractions.IParticipationService s) =>
        //                    s.GetContestParticipationAsync(login, contestId));

        return builder;
    }

    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("user");

        group.MapPost("authorize",
                      (AuthorizationModel model, HttpContext context, IAuthenticationService service) =>
                          service.AuthenticateUserAsync(context, model.Code));

        group.MapGet("current",
                     (HttpContext context, IUserService userService) =>
                         userService.GetUserAsync(context.GetUserId()))
             .RequireAuthorization();

        return builder;
    }

    /*
    public static IEndpointRouteBuilder MapAuthorizationEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("authorize");

        group.MapGet("url",
                     async (string? redirectUrl,
                            [FromServices] IYandexIdUserServiceClient c) => TypedResults.Ok(await c.GetOAuthUrlAsync(redirectUrl)));

        group.MapGet("redirect",
                     async (string? redirectUrl,
                            [FromServices] IYandexIdUserServiceClient c) =>
                         TypedResults.Redirect(await c.GetOAuthUrlAsync(redirectUrl)));

        return builder;
    }
    */
}
