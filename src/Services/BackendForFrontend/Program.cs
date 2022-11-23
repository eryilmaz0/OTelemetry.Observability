using Core.Client;
using Core.Metric;
using Core.Model;
using Core.Model.OptionModel;
using Core.Tracing;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<INotificationServiceClient, NotificationServiceClient>(config =>
{
    config.BaseAddress = new Uri(builder.Configuration.GetValue<string>("NotificationServiceUrl"));
});


builder.Services.AddHttpClient<ICustomerServiceClient, CustomerServiceClient>(config =>
{
    config.BaseAddress = new Uri(builder.Configuration.GetValue<string>("CustomerServiceUrl"));
});

/*builder.Services.AddOpenTelemetryMetrics(config =>
{
    config.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Backend For Frontend", serviceVersion:"1.0"));
    config.AddMeter("Metrics")
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()    
        .AddHttpClientInstrumentation()
        .AddConsoleExporter()
        .AddOtlpExporter(opt =>
        {
            string otlpEndpoint = "http://otel-collector:4317";
            opt.Endpoint = new Uri(otlpEndpoint);
            opt.Protocol = OtlpExportProtocol.Grpc;
        });
}); */

//TracingOptions tracingOptions = builder.Configuration.GetSection("TracingOptions").Get<TracingOptions>();
//builder.Services.AddTracingSupport(tracingOptions);


builder.Services.AddOpenTelemetryMetrics(config =>
{
    config.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Backend For Frontend", serviceVersion:"1.0"));
    config.AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter()
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri("http://otel-collector:4317");
            options.Protocol = OtlpExportProtocol.Grpc;
        });
}); 


builder.Services.AddOpenTelemetryTracing(config => config
    .AddSource("Backend For Frontend")
    .AddSource("Performance Metric")
    .AddSource("Custom")
    .AddSource("MassTransit")
    .AddConsoleExporter()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName: "Backend For Frontend", serviceVersion: "1.0"))
    .AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://otel-collector:4317");
        options.Protocol = OtlpExportProtocol.Grpc;
    })
    .AddAspNetCoreInstrumentation(options =>
    {
        options.RecordException = true;
        options.Filter = (req) =>
            !req.Request.Path.ToUriComponent().Contains("index.html", StringComparison.OrdinalIgnoreCase) &&
            !req.Request.Path.ToUriComponent().Contains("swagger", StringComparison.OrdinalIgnoreCase);
    })
    .AddHttpClientInstrumentation(options =>
    {
        options.RecordException = true;
    })); 

builder.Services.AddCustomTracerSupport();

/*builder.Services.AddOpenTelemetryTracing(config => config
    .AddSource("Backend For Frontend")
    .AddSource("Performance Metric")
    .AddSource("Custom")
    .AddSource("MassTransit")
    .SetResourceBuilder(
        ResourceBuilder.CreateDefault().AddService(serviceName: "Backend For Frontend", serviceVersion: "1.0"))
    .AddOtlpExporter(options =>
    {
        string otlpEndpoint = "http://otel-collector:4317";
        options.Endpoint = new Uri(otlpEndpoint);
        options.Protocol = OtlpExportProtocol.Grpc;
    })
    .AddAspNetCoreInstrumentation(options =>
    {
        options.RecordException = true;
        options.Filter = (req) =>
            !req.Request.Path.ToUriComponent().Contains("index.html", StringComparison.OrdinalIgnoreCase) &&
            !req.Request.Path.ToUriComponent().Contains("swagger", StringComparison.OrdinalIgnoreCase);
    })
    .AddHttpClientInstrumentation(options =>
    {
        options.RecordException = true;
    })); */




var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();