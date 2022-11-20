namespace Notification.API.Cache;

public interface ICacheProxy
{
    Task AddAsync(Model.Notification notification);
}