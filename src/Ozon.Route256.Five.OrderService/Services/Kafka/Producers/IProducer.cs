namespace Ozon.Route256.Five.OrderService.Services.Kafka.Producers;

public interface IProducer<in TMessage>
{
    public Task Send(TMessage message, CancellationToken cancellationToken);

    public static virtual string ProducerName => nameof(TMessage);
}