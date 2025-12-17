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
                .AddScoped<IRegionService, RegionService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IParticipationService, ParticipationService>()
                .AddScoped<IResultService, ResultService>();
}
