using Texnokaktus.ProgOlymp.Api.Services;
using Texnokaktus.ProgOlymp.Api.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Extensions;

internal static class DiExtensions
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services) =>
        services.AddScoped<IAuthenticationService, JwtAuthenticationService>()
                .AddScoped<IRegistrationService, RegistrationService>();
}
