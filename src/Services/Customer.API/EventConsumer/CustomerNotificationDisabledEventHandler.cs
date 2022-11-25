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
        var customerNotificaticationsDisabledEvent = context.Message;

        Dictionary<string, string> eventPublishMetrics = new()
        {
            { "HandledEvent", JsonSerializer.Serialize(customerNotificaticationsDisabledEvent)},
            { "EventType", customerNotificaticationsDisabledEvent.GetType().Name}
        };
        _tracer.Trace(OperationType.HandleEvent, "Handled Event!", eventPublishMetrics);
        _logger.LogInformation($"Handled Event! {JsonSerializer.Serialize(@customerNotificaticationsDisabledEvent)}", customerNotificaticationsDisabledEvent);
        
        var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Email == customerNotificaticationsDisabledEvent.Email);
        customer.NotificationsDisabled = true;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Customer Notifications Disabled", customerNotificaticationsDisabledEvent);
    }
}