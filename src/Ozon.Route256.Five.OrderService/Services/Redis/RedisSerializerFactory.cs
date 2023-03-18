using Google.Protobuf;
using StackExchange.Redis;

namespace Ozon.Route256.Five.OrderService.Services.Redis;

/// <summary>
/// Фабрика сериализаторов
/// Если доступна Protobuf сериализация берем ее
/// Если нет берм Json
/// </summary>
public class RedisSerializerFactory<T> : IRedisSerializer<T> where T : IMessage<T>
{
    private readonly IRedisSerializer<T> _redisSerializer;

    public RedisSerializerFactory(IServiceProvider serviceProvider)
    {
        var targetType = typeof(T);
        var imessageOfT = typeof(IMessage<>)
            .MakeGenericType(targetType);
        if (targetType.IsAssignableTo(imessageOfT))
        {
            var parser = targetType
                             .GetProperty("Parser")
                             ?.GetValue(null)
                         ?? throw new NotSupportedException($"{targetType.Name} не содержит свойства Parser");
            
            var grpcType = typeof(RedisProtoSerializer<>).MakeGenericType(targetType);
            
            _redisSerializer = (IRedisSerializer<T>)(Activator.CreateInstance(grpcType, parser)
                                                     ?? throw new NotSupportedException(
                                                         "Не удалось создать сериализатор"));
        }
        else
            _redisSerializer = ActivatorUtilities.CreateInstance<RedisJsonSerializer<T>>(serviceProvider);
    }

    public RedisValue Serialize(T? value)
        => _redisSerializer.Serialize(value);

    public T? Deserialize(RedisValue value)
        => _redisSerializer.Deserialize(value);
}