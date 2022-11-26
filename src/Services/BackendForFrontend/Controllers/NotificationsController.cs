using System.Diagnostics;
using Core.Client;
using Core.Model.Client;
using Core.Tracing;
using Microsoft.AspNetCore.Mvc;

namespace BackendForFrontend.Controllers;

[ApiController]
[Route("api/Notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationServiceClient _notificationServiceClient;
    private readonly ICustomTracer _tracer;
    private readonly ILogger<NotificationsController> _logger;
    public NotificationsController(INotificationServiceClient notificationServiceClient, ICustomTracer tracer, ILogger<NotificationsController> logger)
    {
        _notificationServiceClient = notificationServiceClient;
        _tracer = tracer;
        _logger = logger;
    }
    
    
    [HttpPost]
    public async Task<IActionResult> DisableCustomerNotifications(DisableCustomerNotificationModel request)
    {
        Stopwatch timer = new();
        timer.Start();
        
        _logger.LogInformation("Disable Customer Notifications Request Started");
        var result = await _notificationServiceClient.DisableCustomerNotificationsAsync(request);
        
        timer.Stop();
        var executionTime = timer.ElapsedMilliseconds;
        
        Dictionary<string, string> executionMetrics = new()
        {
            { "Action Name", "Disable Customer Notification"},
            { "Execution Time", executionTime.ToString()}
        };
        
        _tracer.Trace(OperationType.ActionExecution, "Action Executed!", executionMetrics);
        _logger.LogInformation("Disable Customer Notifications Request Completed");
        
        if (!result.Succeed)
            return BadRequest(result);
        return Ok(result);
    }
}