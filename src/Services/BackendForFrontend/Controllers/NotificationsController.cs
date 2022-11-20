using Core.Client;
using Core.Model.Client;
using Microsoft.AspNetCore.Mvc;

namespace BackendForFrontend.Controllers;

[ApiController]
[Route("api/Notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationServiceClient _notificationServiceClient;

    public NotificationsController(INotificationServiceClient notificationServiceClient)
    {
        _notificationServiceClient = notificationServiceClient;
    }
    
    
    [HttpPost]
    public async Task<IActionResult> DisableCustomerNotifications(DisableCustomerNotificationModel request)
    {
        var result = await _notificationServiceClient.DisableCustomerNotificationsAsync(request);
        return Ok(result);
    }
}