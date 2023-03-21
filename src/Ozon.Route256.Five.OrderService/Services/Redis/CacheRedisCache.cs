using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Ozon.Route256.Five.OrderService.Services.Redis;

public partial class CacheRedisCache : IRedisCache
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CacheRedisCache> _logger;
    private readonly IConnectionMultiplexer _multiplexer;
    private readonly CacheOptions _cacheOptions;
    
    public CacheRedisCache(
        IServiceProvider sp,
        ILogger<CacheRedisCache> logger,
        IConnectionMultiplexer multiplexer,
        IOptions<CacheOptions> cacheOptions)
    {
        _serviceProvider = sp;
        _logger = logger;
        _multiplexer = multiplexer;
        _cacheOptions = cacheOptions.Value;
    }

    protected IDatabase Database => _multiplexer.GetDatabase(0);

    protected string GetKey<T>(string key) => $"cache:{typeof(T).Name}:{key}";
    
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

    protected IRedisSerializer<T> GetSerializer<T>() 
        => _serviceProvider.GetRequiredService<IRedisSerializer<T>>();
    
    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> getter,
        TimeSpan? ttl,
        CancellationToken cancellationToken)
    {
        T? result;
        ttl ??= _cacheOptions.GetCacheTtlForType<T>();
        
        try
        {
            result = await GetAsync<T>(key, cancellationToken);
        
            if (result != null)
                return result;
        }
        catch (Exception e)
        {
            RedisCacheOperationFailed(e);
        }

        result = await getter();

        try
        {
            await SetAsync(key, result, ttl.Value, cancellationToken);
        }
        catch (Exception e)
        {
            RedisCacheOperationFailed(e);
        }

        return result;
    }

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Warning,
        Message = "Поптыка обратится к кэшу не удалась, игнорируем кэш")]
    public partial void RedisCacheOperationFailed(Exception ex);
}