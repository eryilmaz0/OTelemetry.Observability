namespace Customer.API.Model;

public class Customer
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public DateTime Created { get; set; }
}