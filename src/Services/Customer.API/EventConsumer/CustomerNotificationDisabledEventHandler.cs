using Core.Event;
using Customer.API.Context;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Customer.API.EventConsumer;

public class CustomerNotificationDisabledEventHandler : IConsumer<CustomerNotificationsDisabledEvent>
{
    private readonly AppDbContext _context;

    public CustomerNotificationDisabledEventHandler(AppDbContext context)
    {
        _context = context;
    }
    
    
    public async Task Consume(ConsumeContext<CustomerNotificationsDisabledEvent> context)
    {
        var @event = context.Message;
        var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Email == @event.Email);
        customer.NotificationsDisabled = false;
        await _context.SaveChangesAsync();
    }
}