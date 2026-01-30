// src/common/Observability/TelemetryExtensions.cs
using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
// ✅ Add this namespace so AddRuntimeInstrumentation() is visible
using OpenTelemetry.Instrumentation.Runtime;

namespace Common.Observability
{
    public static class TelemetryExtensions
    {
        public static IServiceCollection AddDefaultTelemetry(this IServiceCollection services, string serviceName, IConfiguration cfg)
        {
            var builder = services
                .AddOpenTelemetry()
                .ConfigureResource(rb => rb.AddService(serviceName));

            // Metrics: ASP.NET Core + Runtime (GC/threads/etc.)
            builder.WithMetrics(m =>
            {
                m.AddAspNetCoreInstrumentation();
                m.AddRuntimeInstrumentation();
            });

            // Traces: ASP.NET Core + HttpClient
            builder.WithTracing(t =>
            {
                t.AddAspNetCoreInstrumentation();
                t.AddHttpClientInstrumentation();
            });

            var cs = cfg["APPINSIGHTS_CONNECTION_STRING"];
            if (!string.IsNullOrWhiteSpace(cs))
            {
                // One-liner enables logs, metrics, traces export to Azure Monitor
                builder.UseAzureMonitor(o => o.ConnectionString = cs);
            }

            return services;
        }
    }
}