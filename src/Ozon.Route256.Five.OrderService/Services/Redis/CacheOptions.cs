using System.ComponentModel.DataAnnotations;

namespace Ozon.Route256.Five.OrderService.Services.Redis;

public class CacheOptions
{
    public Dictionary<string, TimeSpan>? TypeLifetime { get; set; }

    [Required] public TimeSpan DefaultCacheTime { get; set; }

    public TimeSpan GetCacheTtlForType<T>()
        => TypeLifetime?.GetValueOrDefault(typeof(T).Name) ?? DefaultCacheTime;
}