namespace Ozon.Route256.Five.OrderService.Services.Redis;

public interface IRedisCache : IRedisStore
{
    Task<T> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> getter,
        TimeSpan? ttl,
        CancellationToken cancellationToken);
    
    Task<T> GetOrSetAsync<T>(
        Func<Task<T>> getter,
        TimeSpan? ttl,
        CancellationToken cancellationToken);
}