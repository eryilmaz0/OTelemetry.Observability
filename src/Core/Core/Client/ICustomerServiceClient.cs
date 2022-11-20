using Core.Model.Client;

namespace Core.Client;

public interface ICustomerServiceClient
{
    Task<ClientResponse> CreateCustomerAsync(CreateCustomerModel requestModel);
}