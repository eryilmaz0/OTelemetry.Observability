using System.Text.Json;
using Core.Event;
using Core.Model.Client;
using Core.Tracing;
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
    private readonly ICustomTracer _tracer;

    public NotificationsController(ICacheProxy cacheProxy, IPublishEndpoint eventPublisher, ICustomTracer tracer)
    {
        _cacheProxy = cacheProxy;
        _eventPublisher = eventPublisher;
        _tracer = tracer;
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
        
        Dictionary<string, string> eventPublishMetrics = new()
        {
            { "PublishingEvent", JsonSerializer.Serialize(@event) },
            { "EventType", @event.GetType().Name }
        };
        _tracer.Trace(OperationType.EventPublish, "Publishing Event!", eventPublishMetrics);
        
        await _eventPublisher.Publish(@event);
        return Ok(new{Status = 200, Message="Customer Notifications Disabled."});
    }
}