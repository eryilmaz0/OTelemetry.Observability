using System.Text.Json;
using Core.Event;
using Core.Tracing;
using MassTransit;
using Notification.API.Cache;

namespace Notification.API.EventConsumer;

public class CustomerCreatedEventHandler : IConsumer<CustomerCreatedEvent>
{
    private readonly ICacheProxy _cacheProxy;
    private readonly ICustomTracer _tracer;
    private readonly ILogger<CustomerCreatedEventHandler> _logger;

    public CustomerCreatedEventHandler(ICacheProxy cacheProxy, ICustomTracer tracer, ILogger<CustomerCreatedEventHandler> logger)
    {
        _cacheProxy = cacheProxy;
        _tracer = tracer;
        _logger = logger;
    }
    
    
    public async Task Consume(ConsumeContext<CustomerCreatedEvent> context)
    {
        var createdUser = context.Message;
        
        Dictionary<string, string> eventPublishMetrics = new()
        {
            { "HandledEvent", JsonSerializer.Serialize(createdUser)},
            { "EventType", createdUser.GetType().Name}
        };

        _tracer.Trace(OperationType.HandleEvent, "Handled Event!", eventPublishMetrics);
        _logger.LogInformation("Handled Event!", @createdUser);
        Model.Notification newNotification = new()
        {
            Email = createdUser.Email,
            NotificationType = "Customer Created"
        };

        await _cacheProxy.AddAsync(newNotification);
        _logger.LogInformation("Customer Created Notification Added", @createdUser);
    }
}