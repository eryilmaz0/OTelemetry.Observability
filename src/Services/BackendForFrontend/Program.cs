using Core.Client;
using Core.Metric;
using Core.Model;
using Core.Model.OptionModel;
using Core.Tracing;

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

//builder.AddMetricSupport();

TracingOptions tracingOptions = builder.Configuration.GetSection("TracingOptions").Get<TracingOptions>();
builder.Services.AddTracingSupport(tracingOptions);
builder.Services.AddCustomTracerSupport();




var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();