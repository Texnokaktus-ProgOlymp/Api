using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Texnokaktus.ProgOlymp.Api.DataAccess.Services.Abstractions;
using Texnokaktus.ProgOlymp.Api.Logic.Observability;

namespace Texnokaktus.ProgOlymp.Api.Logic.Hosted;

public class MetricsInitService(IServiceProvider serviceProvider) : BackgroundService
{
    /*
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var authenticatedUsersCounter = MeterProvider.Meter.CreateAuthenticatedUsersCounter();
        var registeredUsersCounter = MeterProvider.Meter.CreateRegisteredUsersCounter();

        foreach (var user in await unitOfWork.UserRepository.GetUsersAsync())
            authenticatedUsersCounter.Add(1, KeyValuePair.Create<string, object?>("login", user.Login));

        foreach (var application in await unitOfWork.ApplicationRepository.GetApplicationsAsync(1))
            registeredUsersCounter.Add(1,
                                       KeyValuePair.Create<string, object?>("contestId", application.ContestId),
                                       KeyValuePair.Create<string, object?>("regionId", application.RegionId));
    }
    */
    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;
}
