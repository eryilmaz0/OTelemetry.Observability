using System.Text.Json;
using Core.Event;
using Core.Model.Client;
using Core.Tracing;
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
    private readonly ICustomTracer _tracer;

    public CustomersController(AppDbContext context, IPublishEndpoint eventPublisher, ICustomTracer tracer)
    {
        _context = context;
        _eventPublisher = eventPublisher;
        _tracer = tracer;
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

        Dictionary<string, string> eventPublishMetrics = new()
        {
            { "PublishingEvent", JsonSerializer.Serialize(@event) },
            { "EventType", @event.GetType().Name }
        };
        _tracer.Trace(OperationType.EventPublish, "Publishing Event!", eventPublishMetrics);
        
        await _eventPublisher.Publish(@event);
        return Ok(new{Status = 200, Message="Customer Added."});
    }
}