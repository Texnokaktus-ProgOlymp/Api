using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Texnokaktus.ProgOlymp.Api.DataAccess.Context;
using Texnokaktus.ProgOlymp.Api.Logic.Observability;

namespace Texnokaktus.ProgOlymp.Api.Logic.Hosted;

public class MetricsInitService(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var authenticatedUsersCounter = MeterProvider.Meter.CreateAuthenticatedUsersCounter();
        var registeredUsersCounter = MeterProvider.Meter.CreateRegisteredUsersCounter();

        await foreach (var login in context.Users
                                            .Select(user => user.Login)
                                            .AsAsyncEnumerable()
                                            .WithCancellation(cancellationToken))
        {
            authenticatedUsersCounter.Add(1, KeyValuePair.Create<string, object?>("login", login));
        }

        await foreach (var application in context.Applications
                                                 .Select(application => new
                                                  {
                                                      application.ContestId,
                                                      application.RegionId
                                                  })
                                                 .AsAsyncEnumerable()
                                                 .WithCancellation(cancellationToken))
        {
            registeredUsersCounter.Add(1,
                                       KeyValuePair.Create<string, object?>("contestId", application.ContestId),
                                       KeyValuePair.Create<string, object?>("regionId", application.RegionId));
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
