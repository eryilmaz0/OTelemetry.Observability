using Core.Event;
using Core.Model.Client;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Notification.API.Cache;

namespace Notification.API.Controllers;

[ApiController]
[Route("api/Notifications")]
public class NotificationsController : ControllerBase
{
    private readonly ICacheProxy _cacheProxy;
    private readonly IPublishEndpoint _eventPublisher;

    public NotificationsController(ICacheProxy cacheProxy, IPublishEndpoint eventPublisher)
    {
        _cacheProxy = cacheProxy;
        _eventPublisher = eventPublisher;
    }

    [HttpPost]
    public async Task<IActionResult> DisableCustomerNotifications(DisableCustomerNotificationModel request)
    {
        var newNotification = new Model.Notification()
        {
            Email = request.Email,
            NotificationType = "Disabled Notifications"
        };

        await _cacheProxy.AddAsync(newNotification);

        CustomerNotificationsDisabledEvent @event = new()
        {
            Email = request.Email,
            Created = DateTime.UtcNow
        };
        
        await _eventPublisher.Publish(@event);
        return Ok();
    }
}