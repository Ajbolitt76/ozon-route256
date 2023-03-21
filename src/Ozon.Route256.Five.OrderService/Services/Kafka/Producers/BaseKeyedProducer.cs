using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Ozon.Route256.Five.OrderService.Services.Kafka.Settings;

namespace Ozon.Route256.Five.OrderService.Services.Kafka.Producers;

public abstract class BaseKeyedProducer<TImplementation, TKey, TMessage> : IMessageProducer<TKey, TMessage>
    where TImplementation : IMessageProducer<TKey, TMessage>
{
    private readonly IKafkaBinaryProducer _binaryProducer;
    private readonly ProducerSettings _producerSettings;

    protected BaseKeyedProducer(IKafkaBinaryProducer kafkaBinaryProducer, IOptions<KafkaOptions> kafkaOptions)
    {
        _binaryProducer = kafkaBinaryProducer;
        _producerSettings = kafkaOptions.Value.ProducerSettings.GetValueOrDefault(TImplementation.ProducerName)
                            ?? throw new InvalidOperationException(
                                $"Не задан конфиг для консьюмера {TImplementation.ProducerName}");
    }

    protected virtual byte[] SerializeKey(TKey key)
        => JsonSerializer.SerializeToUtf8Bytes(key);
    
    protected virtual byte[] SerializeMessage(TMessage message)
        => JsonSerializer.SerializeToUtf8Bytes(message);

    protected virtual Message<byte[], byte[]> GetMessage(TKey key, TMessage message)
        => new()
        {
            Key = SerializeKey(key),
            Value = SerializeMessage(message)
        };

    public Task Send(TKey key, TMessage message, CancellationToken cancellationToken) 
        => _binaryProducer.SendMessage(_producerSettings.Topic, GetMessage(key, message), cancellationToken);
}