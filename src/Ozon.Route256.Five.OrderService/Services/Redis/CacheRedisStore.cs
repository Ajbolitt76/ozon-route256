using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Ozon.Route256.Five.OrderService.Services.Redis;

public partial class CacheRedisStore : BaseRedisStore, IRedisCache
{
    private readonly ILogger<CacheRedisStore> _logger;
    private readonly IConnectionMultiplexer _multiplexer;
    private readonly CacheOptions _cacheOptions;

    public CacheRedisStore(
        IServiceProvider sp,
        ILogger<CacheRedisStore> logger,
        IConnectionMultiplexer multiplexer,
        IOptions<CacheOptions> cacheOptions) : base(sp)
    {
        _logger = logger;
        _multiplexer = multiplexer;
        _cacheOptions = cacheOptions.Value;
    }

    protected override IDatabase Database => _multiplexer.GetDatabase(0);

    protected override string GetKey<T>(string key) => $"cache:{typeof(T).Name}:{key}";
    
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

    public Task<T> GetOrSetAsync<T>(
        Func<Task<T>> getter,
        TimeSpan? ttl,
        CancellationToken cancellationToken)
        => GetOrSetAsync("default", getter, ttl, cancellationToken);
    
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Warning,
        Message = "Поптыка обратится к кэшу не удалась, игнорируем кэш")]
    public partial void RedisCacheOperationFailed(Exception ex);
}