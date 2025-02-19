using System.Diagnostics.Metrics;

namespace Texnokaktus.ProgOlymp.Api.Logic.Observability;

internal static class MeterExtensions
{
    public static Counter<int> CreateAuthenticatedUsersCounter(this Meter meter) => meter.CreateCounter<int>("users.authenticated");
    public static Counter<int> CreateRegisteredUsersCounter(this Meter meter) => meter.CreateCounter<int>("users.registered");
}
