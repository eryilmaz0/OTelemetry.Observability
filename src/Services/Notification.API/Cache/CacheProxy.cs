using System.Text.Json;
using StackExchange.Redis;

namespace Notification.API.Cache;

public class CacheProxy : ICacheProxy
{
    private readonly IConnectionMultiplexer _connection;
    private readonly IDatabase _database;
    
    
    public async Task AddAsync(Model.Notification notification)
    {
        var guidId = Guid.NewGuid().ToString();
        await _database.StringSetAsync(guidId, JsonSerializer.Serialize(notification));
    }
}