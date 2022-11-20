using Core.Model.Client;

namespace Core.Client;

public interface INotificationServiceClient
{
    Task<ClientResponse> DisableCustomerNotificationsAsync(DisableCustomerNotificationModel requestModel);
}