using Prometheus;

namespace Dodo1000Bot.Services.Metrics;

public static class MetricsCollector
{
    private static readonly Gauge Metrics;

    static MetricsCollector()
    {
        Metrics = Prometheus.Metrics
            .CreateGauge("metrics", "Custom metrics", "metric_name");
    }

    public static void Increment(string key)
    {
        Metrics.WithLabels(key).Inc();
    }

    public static void Set(string key, double value)
    {
        Metrics.WithLabels(key).Set(value);
    }
}