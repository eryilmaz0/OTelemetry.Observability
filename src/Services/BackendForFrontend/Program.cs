using BackendForFrontend.Extensions;
using Core.Client;
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

TracingOptions tracingOptions = builder.Configuration.GetSection("TracingOptions").Get<TracingOptions>();
MetricOptions metricOptions = builder.Configuration.GetSection("MetricOptions").Get<MetricOptions>();
LoggingOptions loggingOptions = builder.Configuration.GetSection("LoggingOptions").Get<LoggingOptions>();
builder.Services.AddMetricSupport(metricOptions);
builder.Services.AddTracingSupport(tracingOptions);
builder.Logging.AddLoggingSupport(loggingOptions);
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();