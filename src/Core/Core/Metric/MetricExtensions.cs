using Core.Model.OptionModel;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace Core.Metric;

public static class MetricExtensions
{
    public static IServiceCollection AddMetricSupport(this IServiceCollection services, MetricOptions options)
    {
        services.AddOpenTelemetryMetrics(config =>
        {
            config.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName: options.ServiceName,
                serviceVersion: options.ServiceVersion));
            config.AddMeter("Metrics");

            if (options.AddAspNetCoreMetrics)
                config.AddAspNetCoreInstrumentation();

            if (options.AddRuntimeMetrics)
                config.AddRuntimeInstrumentation();

            if (options.AddHttpClientMetrics)
                config.AddRuntimeInstrumentation();

            config.AddConsoleExporter();
            config.AddOtlpExporter(opt =>
            {
                string otlpEndpoint = options.OtlpExportUrl;
                opt.Endpoint = new Uri(otlpEndpoint);
                opt.Protocol = OtlpExportProtocol.Grpc;
            }); 
        });

        return services;
    }
}