namespace Notification.API.Model;

public class Notification
{
    public string Email { get; set; }
    public string NotificationType { get; set; }
    public bool IsSent { get; set; } = false;
}