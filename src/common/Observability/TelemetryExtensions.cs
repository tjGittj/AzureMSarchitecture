
using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Common.Observability;

public static class TelemetryExtensions
{
    public static IServiceCollection AddDefaultTelemetry(this IServiceCollection services, string serviceName, IConfiguration cfg)
    {
        var resource = ResourceBuilder.CreateDefault().AddService(serviceName);
        var builder = services.AddOpenTelemetry().ConfigureResource(rb => rb.AddService(serviceName));
        builder.WithMetrics(m => m.AddAspNetCoreInstrumentation().AddRuntimeInstrumentation());
        builder.WithTracing(t => { t.AddAspNetCoreInstrumentation(); t.AddHttpClientInstrumentation(); });

        var cs = cfg["APPINSIGHTS_CONNECTION_STRING"];
        if (!string.IsNullOrWhiteSpace(cs))
        {
            builder.UseAzureMonitor(o => o.ConnectionString = cs);
        }
        return services;
    }
}
