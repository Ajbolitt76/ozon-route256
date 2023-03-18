using Confluent.Kafka;

namespace Ozon.Route256.Five.OrderService.Services.Kafka.Producers;

public interface IKafkaBinaryProducer
{
    Task SendMessage(string topic, Message<byte[], byte[]> message, CancellationToken token);
}