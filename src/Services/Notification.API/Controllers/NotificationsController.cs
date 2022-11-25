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
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(ICacheProxy cacheProxy, IPublishEndpoint eventPublisher, ICustomTracer tracer, ILogger<NotificationsController> logger)
    {
        _cacheProxy = cacheProxy;
        _eventPublisher = eventPublisher;
        _tracer = tracer;
        _logger = logger;
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
        _logger.LogInformation("Disable Notification Record Added.");

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
        _logger.LogInformation($"Event Published! {JsonSerializer.Serialize(eventPublishMetrics)}");
        
        await _eventPublisher.Publish(@event);
        return Ok(new{Status = 200, Message="Customer Notifications Disabled."});
    }
}