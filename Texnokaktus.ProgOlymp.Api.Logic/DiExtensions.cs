using Microsoft.Extensions.DependencyInjection;
using Texnokaktus.ProgOlymp.Api.Logic.Hosted;
using Texnokaktus.ProgOlymp.Api.Logic.Services;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic;

public static class DiExtensions
{
    public static IServiceCollection AddLogicServices(this IServiceCollection services) =>
        services.AddHostedService<MetricsInitService>()
                .AddScoped<IRegistrationService, RegistrationService>()
                .AddScoped<IContestService, ContestService>()
                .Decorate<IContestService, ContestServiceCachingDecorator>()
                .AddScoped<IRegionService, RegionService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IParticipationService, ParticipationService>()
                .Decorate<IParticipationService, ParticipationServiceCachingDecorator>()
                .AddScoped<IResultService, ResultService>();
}
