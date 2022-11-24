using System.Text.Json;
using Core.Event;
using Core.Tracing;
using Customer.API.Context;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Customer.API.EventConsumer;

public class CustomerNotificationDisabledEventHandler : IConsumer<CustomerNotificationsDisabledEvent>
{
    private readonly AppDbContext _context;
    private readonly ICustomTracer _tracer;
    private readonly ILogger<CustomerNotificationDisabledEventHandler> _logger;

    public CustomerNotificationDisabledEventHandler(AppDbContext context, ICustomTracer tracer, ILogger<CustomerNotificationDisabledEventHandler> logger)
    {
        _context = context;
        _tracer = tracer;
        _logger = logger;
    }
    
    
    public async Task Consume(ConsumeContext<CustomerNotificationsDisabledEvent> context)
    {
        var @event = context.Message;

        Dictionary<string, string> eventPublishMetrics = new()
        {
            { "HandledEvent", JsonSerializer.Serialize(@event)},
            { "EventType", @event.GetType().Name}
        };
        _tracer.Trace(OperationType.HandleEvent, "Handled Event!", eventPublishMetrics);
        _logger.LogInformation("Handled Event!", @event);
        
        var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Email == @event.Email);
        customer.NotificationsDisabled = true;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Customer Notifications Disabled", @event);
    }
}