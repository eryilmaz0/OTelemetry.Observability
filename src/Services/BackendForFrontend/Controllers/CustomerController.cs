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

    public CustomerController(ICustomerServiceClient customerServiceClient, ICustomTracer tracer)
    {
        _customerServiceClient = customerServiceClient;
        _tracer = tracer;
    }
    
    
    [HttpPost]
    public async Task<IActionResult> CreateCustomer(CreateCustomerModel request)
    {
        Stopwatch timer = new();
        timer.Start();
        
        var result = await _customerServiceClient.CreateCustomerAsync(request);
      
        
        timer.Stop();
        var executionTime = timer.ElapsedMilliseconds;
        
        Dictionary<string, string> executionMetrics = new()
        {
            { "Action Name", "Create Customer"},
            { "Execution Time", executionTime.ToString()}
        };
        
        _tracer.Trace(OperationType.ActionExecution, "Action Executed!", executionMetrics);
        
        if (!result.Succeed)
            return BadRequest(result);
        return Ok(result);
    }
}