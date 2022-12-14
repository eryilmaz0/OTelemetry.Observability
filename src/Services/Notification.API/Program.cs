using Core.Model;
using Core.Model.OptionModel;
using Core.Tracing;
using Customer.API.Helper;
using MassTransit;
using Notification.API.Cache;
using Notification.API.EventConsumer;
using Notification.API.Extensions;
using Npgsql;
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

await CacheProxyHelper.ConnectCacheProxy(builder.Services, builder.Configuration);
builder.Services.AddSingleton<ICacheProxy, CacheProxy>();

builder.Services.AddMassTransit(x=>
{
    x.AddConsumer<CustomerCreatedEventHandler>();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetSection("EventBusOptions:HostUrl").Value, host =>
        {
            host.Username(builder.Configuration.GetSection("EventBusOptions:UserName").Value);
            host.Password(builder.Configuration.GetSection("EventBusOptions:Password").Value);
        });
        
        cfg.ReceiveEndpoint(builder.Configuration.GetSection("EventBusOptions:QueueName").Value, consumer =>
        {
            consumer.ConfigureConsumer<CustomerCreatedEventHandler>(context);
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