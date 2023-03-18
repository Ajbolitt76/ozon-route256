using Google.Protobuf;
using StackExchange.Redis;

namespace Ozon.Route256.Five.OrderService.Services.Redis;

/// <summary>
/// Сериализатор использующий protobuf
/// </summary>
public class RedisProtoSerializer<T> : IRedisSerializer<T> where T : IMessage<T>
{
    private readonly MessageParser<T> _parser;

    public RedisProtoSerializer(MessageParser<T> parser)
    {
        _parser = parser;
    }

    public RedisValue Serialize(T? value)
        => value?.ToByteArray();

    public T? Deserialize(RedisValue value)
        => value.HasValue
            ? _parser.ParseFrom(value)
            : default;
}