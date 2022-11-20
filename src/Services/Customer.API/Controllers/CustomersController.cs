using Core.Event;
using Core.Model.Client;
using Customer.API.Context;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Customer.API.Controllers;


[ApiController]
[Route("api/Customers")]
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPublishEndpoint _eventPublisher;

    public CustomersController(AppDbContext context, IPublishEndpoint eventPublisher)
    {
        _context = context;
        _eventPublisher = eventPublisher;
    }
    
    
    [HttpPost]
    public async Task<IActionResult> CreateCustomer(CreateCustomerModel request)
    {
        await _context.Customers.AddAsync(new()
        {
            Email = request.Email,
            Name = request.Name,
            LastName = request.LastName,
            Age = request.Age,
            Created = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        CustomerCreatedEvent @event = new()
        {
            Email = request.Email,
            Name = request.Name,
            LastName = request.LastName,
            Age = request.Age,
            Created = DateTime.UtcNow
        };

        await _eventPublisher.Publish(@event);
        return Ok();
    }
}