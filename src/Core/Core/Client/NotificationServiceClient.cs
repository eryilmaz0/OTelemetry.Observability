using System.Net.Http.Json;
using Core.Model.Client;

namespace Core.Client;

public class NotificationServiceClient : INotificationServiceClient
{
    private readonly HttpClient _client;

    public NotificationServiceClient(HttpClient client)
    {
        _client = client;
    }


    public async Task<ClientResponse> DisableCustomerNotificationsAsync(DisableCustomerNotificationModel requestModel)
    {
        ClientResponse result = new();
        var response = await _client.PostAsJsonAsync<DisableCustomerNotificationModel>("customerNotifications", requestModel);
        result.Succeed = response.IsSuccessStatusCode;
        result.Message = response.IsSuccessStatusCode ? "Customer Notifications Disabled." : "Customer Notifications Not Disabled.";
        return result;
    }
}