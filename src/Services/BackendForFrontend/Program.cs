using Core.Client;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();