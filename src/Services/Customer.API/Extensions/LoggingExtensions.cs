using Core.Model.OptionModel;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Serilog;

namespace Customer.API.Extensions;

public static class LoggingExtensions
{
    public static ILoggingBuilder AddLoggingSupport(this ILoggingBuilder logger, LoggingOptions options)
    {
        logger.ClearProviders().SetMinimumLevel(LogLevel.Information).AddOpenTelemetry(opt =>
        {
            opt.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName: options.ServiceName, serviceVersion: options.ServiceVersion));
            opt.IncludeFormattedMessage = options.IncludeFormattedMessage;
            opt.IncludeScopes = options.IncludeScopes;
            opt.ParseStateValues = options.ParseStateValues;

            opt.AddConsoleExporter();
            opt.AddOtlpExporter(opt =>
            {
                string otlpEndpoint = options.OtlpExportUrl;
                opt.Endpoint = new Uri(otlpEndpoint);
                opt.Protocol = OtlpExportProtocol.Grpc;
            });
        });
        
        return logger;
    } 
}