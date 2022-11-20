using Core.Client;
using Core.Model.Client;
using Microsoft.AspNetCore.Mvc;

namespace BackendForFrontend.Controllers;

[ApiController]
[Route("api/Customers")]
public class CustomerController : Controller
{
    private readonly ICustomerServiceClient _customerServiceClient;

    public CustomerController(ICustomerServiceClient customerServiceClient)
    {
        _customerServiceClient = customerServiceClient;
    }
    
    
    [HttpPost]
    public async Task<IActionResult> CreateCustomer(CreateCustomerModel request)
    {
        var result = await _customerServiceClient.CreateCustomerAsync(request);
        return Ok(result);
    }
}