using StackExchange.Redis;

namespace Ozon.Route256.Five.OrderService.Services.Redis;

public interface IRedisSerializer<T>
{
    RedisValue Serialize(T? value);
    T? Deserialize(RedisValue value);
}