using Microsoft.AspNetCore.Http.HttpResults;
using Texnokaktus.ProgOlymp.Api.Extensions;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;
using Texnokaktus.ProgOlymp.Api.Models;
using Texnokaktus.ProgOlymp.Api.Services.Abstractions;
using Texnokaktus.ProgOlymp.Api.Validators;
using IRegistrationService = Texnokaktus.ProgOlymp.Api.Services.Abstractions.IRegistrationService;
using IUserService = Texnokaktus.ProgOlymp.Api.Services.Abstractions.IUserService;

namespace Texnokaktus.ProgOlymp.Api.Endpoints;

internal static class EndpointsMapper
{
    public static IEndpointRouteBuilder MapContestEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("contests/{contestName}");

        group.MapGet("", async Task<Results<Ok<ContestRegistrationState>, NotFound>>(string contestName, IContestService contestService) =>
        {
            if (await contestService.GetContestAsync(contestName) is not { } contest)
                return TypedResults.NotFound();

            return TypedResults.Ok(new ContestRegistrationState(contest.Id,
                                                                contest.Name,
                                                                contest.RegistrationStart,
                                                                contest.RegistrationFinish,
                                                                contest.RegistrationState));
        });

        group.MapPost("register",
                      (string contestName, ApplicationInsertModel model, HttpContext context, IRegistrationService service)
                          => service.RegisterUserAsync(contestName, context.GetUserId(), model))
             .AddEndpointFilter<ValidationFilter<ApplicationInsertModel>>()
             .RequireAuthorization();

        group.MapGet("participation",
                     (string contestName, IParticipationService participationService, HttpContext context)
                         => participationService.GetParticipationAsync(context.GetUserId(), contestName))
             .RequireAuthorization();

        return builder;
    }

    public static IEndpointRouteBuilder MapRegionEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("regions", (IRegionService s) => s.GetAllRegionsAsync());

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
}
