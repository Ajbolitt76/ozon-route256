using System.Text.Json;
using Confluent.Kafka;

namespace Ozon.Route256.Five.OrderService.Services.Kafka.Producers;

public class JsonProducer<TKey, TValue>
{
    protected readonly JsonSerializerOptions SerializerOptions = new();

    protected Message<byte[], byte[]> ToMessage(Message<TKey, TValue> message)
    {
        return new Message<byte[], byte[]>()
        {
            Key = JsonSerializer.SerializeToUtf8Bytes(message.Key),
            Value = JsonSerializer.SerializeToUtf8Bytes(message.Value),
            Headers = message.Headers,
            Timestamp = message.Timestamp
        };
    }
}