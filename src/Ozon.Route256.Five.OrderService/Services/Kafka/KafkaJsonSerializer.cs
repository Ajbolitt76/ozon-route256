using System.Text.Json;
using System.Text.Json.Serialization;
using Confluent.Kafka;

namespace Ozon.Route256.Five.OrderService.Services.Kafka;

public class KafkaJsonSerializer<T> : IDeserializer<T>
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public KafkaJsonSerializer(JsonSerializerOptions? jsonSerializerOptions = null)
    {   
        _jsonSerializerOptions = jsonSerializerOptions ?? new JsonSerializerOptions()
        {
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };
    }
    
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        => JsonSerializer.Deserialize<T>(data, _jsonSerializerOptions)!;
}