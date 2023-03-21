namespace Ozon.Route256.Five.OrderService.Services.Kafka.Producers;

public interface IMessageProducer<TKey, in TMessage>
{
    public Task Send(TKey key, TMessage message, CancellationToken cancellationToken);

    public static virtual string ProducerName => nameof(TMessage);
}