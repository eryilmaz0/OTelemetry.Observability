using Core.Model;
using Core.Model.OptionModel;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;

namespace Core.Tracing;

public static class TracingExtension
{
    public static IServiceCollection AddTracingSupport(this IServiceCollection services, TracingOptions options)
    {
        services.AddOpenTelemetryTracing((config) =>
        {
            config.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName: options.ServiceName, serviceVersion: options.ServiceVersion));
            config.AddSource(options.ServiceName);
            config.AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
                options.Enrich = TraceEnricher.AspNetCoreActivityEnrichment;
                options.Filter = (req) => !req.Request.Path.ToUriComponent().Contains("index.html", StringComparison.OrdinalIgnoreCase) && !req.Request.Path.ToUriComponent().Contains("swagger", StringComparison.OrdinalIgnoreCase);
            });

            if (options.AddPerformanceTracing)
                config.AddSource("Performance Metric");

            if (options.AddEventBusTracing)
                config.AddSource("MassTransit");

            if (options.AddCustomSpanTracing)
                config.AddSource("Custom");
            
            if(options.AddHttpClientTracing)
                config.AddHttpClientInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.Enrich = TraceEnricher.HttpActivityEnrichment;
                });

            if (options.AddDatabaseTracing)
                config.AddNpgsql();

            if (options.AddRedisTracing)
            {
                var serviceProvider = services.BuildServiceProvider();
                IConnectionMultiplexer connection = serviceProvider.GetRequiredService<IConnectionMultiplexer>();
                config.AddRedisInstrumentation(connection, opt => opt.FlushInterval = TimeSpan.FromSeconds(1));
            }

            config.AddConsoleExporter();
            config.AddOtlpExporter(configuration =>
            {
                configuration.Endpoint = new Uri(options.OtlpExportUrl);
                configuration.Protocol = OtlpExportProtocol.Grpc;
            });
        });
        
        return services;
    }
}