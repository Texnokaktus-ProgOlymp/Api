using System.Diagnostics.Metrics;

namespace Texnokaktus.ProgOlymp.Api.Logic.Observability;

internal static class MeterProvider
{
    public static readonly Meter Meter = new(Constants.MeterName);
}
