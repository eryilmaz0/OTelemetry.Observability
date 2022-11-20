namespace Core.Event;

public class CustomerNotificationsDisabledEvent : Event
{
    public string Email { get; set; }
}