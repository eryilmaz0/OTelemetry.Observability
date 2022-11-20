using StackExchange.Redis;

namespace Customer.API.Helper;

public static class CacheProxyHelper
{
    public static async Task ConnectCacheProxy(IServiceCollection services, IConfiguration configuration)
    {
        string redisHost = configuration.GetValue<string>("RedisOptions:HostUrl");
        string redisPort = configuration.GetValue<string>("RedisOptions:Port");
        
        var connection = await ConnectionMultiplexer.ConnectAsync($"{redisHost}:{redisPort}");
        services.AddSingleton<IConnectionMultiplexer>(connection);
    }
}