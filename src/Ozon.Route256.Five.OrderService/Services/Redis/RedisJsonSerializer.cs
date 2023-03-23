using System.Text.Json;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Ozon.Route256.Five.OrderService.Services.Redis;

public class RedisJsonSerializer<T> : IRedisSerializer<T>
{
    private readonly JsonSerializerOptions _options;

    public RedisJsonSerializer(IOptions<JsonSerializerOptions> options)
    {
        _options = options.Value;
    }

    public RedisValue Serialize(T? value)
        => value != null 
            ? JsonSerializer.Serialize<T>(value, _options)
            : default;

    public T? Deserialize(RedisValue value)
        => value.HasValue
            ? JsonSerializer.Deserialize<T>(value!, _options)
            : default;
}