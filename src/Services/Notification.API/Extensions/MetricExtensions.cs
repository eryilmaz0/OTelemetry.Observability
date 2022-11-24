using Core.Model.OptionModel;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace Notification.API.Extensions;

public static class MetricExtensions
{
    public static IServiceCollection AddMetricSupport(this IServiceCollection services, MetricOptions options)
    {
        services.AddOpenTelemetryMetrics(config =>
        {
            config.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(options.ServiceName, serviceVersion: options.ServiceVersion));
            config.AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddHttpClientInstrumentation()
                .AddConsoleExporter()
                .AddOtlpExporter(config =>
                {
                    config.Endpoint = new Uri(options.OtlpExportUrl);
                    config.Protocol = OtlpExportProtocol.Grpc;
                });
        });

        return services;
    }
}