using Prometheus;

namespace Dodo1000Bot.Services.Metrics;

public static class MetricsCollector
{
    private static readonly Gauge Metrics;

    static MetricsCollector()
    {
        Metrics = Prometheus.Metrics
            .CreateGauge("metrics", "Custom metrics", "metric_name", "parameter");
    }

    public static void Increment(string key, string value)
    {
        Metrics.WithLabels(key, value).Inc();
    }
}