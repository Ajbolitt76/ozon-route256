using StackExchange.Redis;

namespace Ozon.Route256.Five.OrderService.Services.Redis;

public static class RedisServiceCollectionsExtensions
{
    public static IServiceCollection AddRedis(this IServiceCollection sc, IConfiguration configuration)
    {
        sc.AddSingleton<IConnectionMultiplexer>(
            sp => ConnectionMultiplexer.Connect(
                configuration.GetConnectionString("RedisConnection")
                ?? throw new ApplicationException("Не указанно подключение к Redis")));

        sc.AddScoped(typeof(IRedisCache), typeof(CacheRedisStore));
        sc.AddScoped(typeof(IRedisSerializer<>), typeof(RedisSerializerFactory<>));
        
        sc.AddOptions<CacheOptions>()
            .Bind(configuration.GetSection(nameof(CacheOptions)))
            .ValidateOnStart()
            .ValidateDataAnnotations();

        return sc;
    }
}