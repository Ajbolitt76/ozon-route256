using StackExchange.Redis;

namespace Ozon.Route256.Five.OrderService.Services.Redis;

public abstract class BaseRedisStore : IRedisStore
{
    private readonly IServiceProvider _serviceProvider;

    public BaseRedisStore(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
    {
        var result = await Database.StringGetAsync(GetKey<T>(key)).WaitAsync(cancellationToken);
        return result.HasValue
            ? GetSerializer<T>().Deserialize(result)
            : default;
    }

    public Task<bool> SetAsync<T>(string key, T value, TimeSpan expiry, CancellationToken cancellationToken) 
        => Database
        .StringSetAsync(GetKey<T>(key), GetSerializer<T>().Serialize(value), expiry)
        .WaitAsync(cancellationToken);

    public Task SetAsync<T>(string key, T value, CancellationToken cancellationToken) 
        => Database
        .StringSetAsync(GetKey<T>(key), GetSerializer<T>().Serialize(value))
        .WaitAsync(cancellationToken);

    public Task DeleteAsync<T>(string key, CancellationToken cancellationToken)
        => Database.KeyDeleteAsync(GetKey<T>(key)).WaitAsync(cancellationToken);

    public Task<bool> ExistsAsync<T>(string key, CancellationToken cancellationToken)
        => Database.KeyExistsAsync(GetKey<T>(key)).WaitAsync(cancellationToken);

    protected abstract IDatabase Database { get; }

    protected abstract string GetKey<T>(string key);

    protected IRedisSerializer<T> GetSerializer<T>() 
        => _serviceProvider.GetRequiredService<IRedisSerializer<T>>();
}