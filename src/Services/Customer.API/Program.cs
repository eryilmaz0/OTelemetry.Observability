using Core.Event;
using Core.Model;
using Core.Model.OptionModel;
using Core.Tracing;
using Customer.API.Context;
using Customer.API.EventConsumer;
using Customer.API.Extensions;
using MassTransit;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddCustomTracerSupport();

builder.Services.AddMassTransit(x=>
{
    x.AddConsumer<CustomerNotificationDisabledEventHandler>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetSection("EventBusOptions:HostUrl").Value, host =>
        {
            host.Username(builder.Configuration.GetSection("EventBusOptions:UserName").Value);
            host.Password(builder.Configuration.GetSection("EventBusOptions:Password").Value);
        });
        
        cfg.ReceiveEndpoint(builder.Configuration.GetSection("EventBusOptions:QueueName").Value, consumer =>
        {
            consumer.ConfigureConsumer<CustomerNotificationDisabledEventHandler>(context);
        });
    });
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