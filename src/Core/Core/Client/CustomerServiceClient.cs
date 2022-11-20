using System.Net.Http.Json;
using Core.Model.Client;

namespace Core.Client;

public class CustomerServiceClient : ICustomerServiceClient
{
    private readonly HttpClient _client;

    public CustomerServiceClient(HttpClient client)
    {
        _client = client;
    }
    
    
    public async Task<ClientResponse> CreateCustomerAsync(CreateCustomerModel requestModel)
    {
        ClientResponse result = new();
        var response = await _client.PostAsJsonAsync<CreateCustomerModel>("customers", requestModel);

        result.Succeed = response.IsSuccessStatusCode;
        result.Message = response.IsSuccessStatusCode ? "Customer Created." : "Customer Could Not Created.";
        return result;
    }
}