using System.Diagnostics.Metrics;

namespace Texnokaktus.ProgOlymp.Api.Logic.Observability;

internal static class MeterExtensions
{
    extension(Meter meter)
    {
        public Counter<int> CreateAuthenticatedUsersCounter() => meter.CreateCounter<int>("users.authenticated");
        public Counter<int> CreateRegisteredUsersCounter() => meter.CreateCounter<int>("users.registered");
    }
}
