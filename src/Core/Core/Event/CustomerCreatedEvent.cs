namespace Core.Event;

public class CustomerCreatedEvent : Event
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
}