using System.Diagnostics;
using Core.Client;
using Core.Model.Client;
using Core.Tracing;
using Microsoft.AspNetCore.Mvc;

namespace BackendForFrontend.Controllers;

[ApiController]
[Route("api/Customers")]
public class CustomerController : Controller
{
    private readonly ICustomerServiceClient _customerServiceClient;
    private readonly ICustomTracer _tracer;
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(ICustomerServiceClient customerServiceClient, ICustomTracer tracer, ILogger<CustomerController> logger)
    {
        _customerServiceClient = customerServiceClient;
        _tracer = tracer;
        _logger = logger;
    }
    
    
    [HttpPost]
    public async Task<IActionResult> CreateCustomer(CreateCustomerModel request)
    {
        Stopwatch timer = new();
        timer.Start();
        
        _logger.LogInformation("Create Customer Request Started");
        var result = await _customerServiceClient.CreateCustomerAsync(request);
      
        
        timer.Stop();
        var executionTime = timer.ElapsedMilliseconds;
        
        Dictionary<string, string> executionMetrics = new()
        {
            { "Action Name", "Create Customer"},
            { "Execution Time", executionTime.ToString()}
        };
        
        _tracer.Trace(OperationType.ActionExecution, "Action Executed!", executionMetrics);
        _logger.LogInformation("Create Customer Request Completed.");
        if (!result.Succeed)
            return BadRequest(result);
        return Ok(result);
    }
}