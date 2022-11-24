using Core.Model.OptionModel;
using Core.Tracing;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Customer.API.Extensions;

public static class TracingExtensions
{
    public static IServiceCollection AddTracingSupport(this IServiceCollection services, TracingOptions options)
    {
        services.AddOpenTelemetryTracing(config => config
            .AddSource(options.ServiceName)
            .AddSource("Performance Metric")
            .AddSource("Custom")
            .AddSource("MassTransit")
            .AddConsoleExporter()
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(serviceName: options.ServiceName, serviceVersion: options.ServiceVersion))
            .AddOtlpExporter(config =>
            {
                config.Endpoint = new Uri(options.OtlpExportUrl);
                config.Protocol = OtlpExportProtocol.Grpc;
            })
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
                options.Enrich = TraceEnricher.AspNetCoreActivityEnrichment;
                options.Filter = (req) =>
                    !req.Request.Path.ToUriComponent().Contains("index.html", StringComparison.OrdinalIgnoreCase) &&
                    !req.Request.Path.ToUriComponent().Contains("swagger", StringComparison.OrdinalIgnoreCase);
            })
            .AddNpgsql());

        services.AddCustomTracerSupport();
        return services;
    }
}