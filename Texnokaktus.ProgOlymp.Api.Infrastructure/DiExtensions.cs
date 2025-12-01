using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients;
using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexId;
using ParticipantService = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ParticipantService;

namespace Texnokaktus.ProgOlymp.Api.Infrastructure;

public static class DiExtensions
{
    public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpcClient<ContestDataService.ContestDataServiceClient>(options => options.Address = configuration.GetConnectionStringUri(nameof(ContestDataService)));
        services.AddGrpcClient<ParticipantService.ParticipantServiceClient>(options => options.Address = configuration.GetConnectionStringUri(nameof(ParticipantService)));
        services.AddGrpcClient<RegistrationService.RegistrationServiceClient>(options => options.Address = configuration.GetConnectionStringUri(nameof(RegistrationService)));
        services.AddGrpcClient<UserService.UserServiceClient>(options => options.Address = configuration.GetConnectionStringUri(nameof(UserService)));

        return services.AddScoped<IContestDataServiceClient, ContestDataServiceClient>()
                       .AddScoped<IRegistrationServiceClient, RegistrationServiceClient>()
                       .AddScoped<IYandexIdUserServiceClient, YandexIdUserServiceClient>();
    }

    private static Uri? GetConnectionStringUri(this IConfiguration configuration, string name) =>
        configuration.GetConnectionString(name) is { } connectionString
            ? new Uri(connectionString)
            : null;
}
