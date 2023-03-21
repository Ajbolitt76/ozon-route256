using Google.Protobuf;
using StackExchange.Redis;

namespace Ozon.Route256.Five.OrderService.Services.Redis;

/// <summary>
/// Фабрика сериализаторов
/// Если доступна Protobuf сериализация берем ее
/// Если нет берм Json
/// </summary>
public class UniversalRedisSerializer<T> : IRedisSerializer<T> where T : IMessage<T>
{
    private readonly IRedisSerializer<T> _redisSerializer;

    public UniversalRedisSerializer(IServiceProvider serviceProvider)
    {
        var targetType = typeof(T);
        var imessageOfT = typeof(IMessage<>).MakeGenericType(targetType);

        if (targetType.IsAssignableTo(imessageOfT))
        {
            var grpcSerializerType = typeof(RedisProtoSerializer<>).MakeGenericType(targetType);
            
            _redisSerializer = (IRedisSerializer<T>) serviceProvider.GetRequiredService(grpcSerializerType);
        }
        else
            _redisSerializer = serviceProvider.GetRequiredService<RedisJsonSerializer<T>>();
    }

    public RedisValue Serialize(T? value)
        => _redisSerializer.Serialize(value);

    public T? Deserialize(RedisValue value)
        => _redisSerializer.Deserialize(value);
}