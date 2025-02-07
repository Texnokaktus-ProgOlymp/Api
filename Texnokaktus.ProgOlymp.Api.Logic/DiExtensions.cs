using Microsoft.Extensions.DependencyInjection;
using Texnokaktus.ProgOlymp.Api.Logic.Services;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic;

public static class DiExtensions
{
    public static IServiceCollection AddLogicServices(this IServiceCollection services) =>
        services.AddScoped<IRegistrationService, RegistrationService>()
                .AddScoped<IContestService, ContestService>()
                .Decorate<IContestService, ContestServiceCachingDecorator>()
                .AddScoped<IRegionService, RegionService>()
                .Decorate<IRegionService, RegionServiceCachingDecorator>()
                .AddScoped<IUserService, UserService>();
}
