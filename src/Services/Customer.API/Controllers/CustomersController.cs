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
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(AppDbContext context, IPublishEndpoint eventPublisher, ICustomTracer tracer, ILogger<CustomersController> logger)
    {
        _context = context;
        _eventPublisher = eventPublisher;
        _tracer = tracer;
        _logger = logger;
    }
    
    
    [HttpPost]
    public async Task<IActionResult> CreateCustomer(CreateCustomerModel request)
    {
        try
        {
            if (request.Age <= 0)
                throw new ArgumentException("Age Can Not Be <= 0.");

            _logger.LogInformation($"Current TraceId Check : {_tracer.GetCurrentTraceId()}");
            await Task.Delay(3000);
            _logger.LogInformation($"Current TraceId Second Check: {_tracer.GetCurrentTraceId()}");
            await _context.Customers.AddAsync(new()
            {
                Email = request.Email,
                Name = request.Name,
                LastName = request.LastName,
                Age = request.Age,
                Created = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            _logger.LogInformation("Customer Added");

            CustomerCreatedEvent customerCreatedEvent = new()
            {
                Email = request.Email,
                Name = request.Name,
                LastName = request.LastName,
                Age = request.Age,
                Created = DateTime.UtcNow
            };

            Dictionary<string, string> eventPublishMetrics = new()
            {
                { "PublishingEvent", JsonSerializer.Serialize(customerCreatedEvent) },
                { "EventType", customerCreatedEvent.GetType().Name }
            };
            _tracer.Trace(OperationType.EventPublish, "Publishing Event!", eventPublishMetrics);

            await _eventPublisher.Publish(customerCreatedEvent);
            _logger.LogInformation($"Event Published! {JsonSerializer.Serialize(customerCreatedEvent)}", customerCreatedEvent);
            return Ok(new{Status = 200, Message="Customer Added."});
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.StackTrace);
            return BadRequest(e.Message);
        }
    }
}