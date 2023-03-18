namespace Ozon.Route256.Five.OrderService.Services.Kafka.Consumers;

public interface IKafkaConsumerHandler<in TKey, in TValue>
{
    public Task Handle(TKey key, TValue message, CancellationToken token);

    public static abstract string ConsumerName { get; }
}
