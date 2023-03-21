namespace Ozon.Route256.Five.OrderService.Services.Redis;

public interface IRedisCache
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken);
    
    Task<bool> SetAsync<T>(string key, T value, TimeSpan expiry, CancellationToken cancellationToken);
    
    Task SetAsync<T>(string key, T value, CancellationToken cancellationToken);
    
    Task DeleteAsync<T>(string key, CancellationToken cancellationToken);
    
    Task<bool> ExistsAsync<T>(string key, CancellationToken cancellationToken);
    
    Task<T> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> getter,
        TimeSpan? ttl,
        CancellationToken cancellationToken);
}