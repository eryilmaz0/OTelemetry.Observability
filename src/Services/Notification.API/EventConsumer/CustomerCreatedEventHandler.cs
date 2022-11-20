using Core.Event;
using MassTransit;
using Notification.API.Cache;

namespace Notification.API.EventConsumer;

public class CustomerCreatedEventHandler : IConsumer<CustomerCreatedEvent>
{
    private readonly ICacheProxy _cacheProxy;

    public CustomerCreatedEventHandler(ICacheProxy cacheProxy)
    {
        _cacheProxy = cacheProxy;
    }
    
    
    public async Task Consume(ConsumeContext<CustomerCreatedEvent> context)
    {
        var createdUser = context.Message;
        Model.Notification newNotification = new()
        {
            Email = createdUser.Email,
            NotificationType = "Customer Created"
        };

        await _cacheProxy.AddAsync(newNotification);
    }
}