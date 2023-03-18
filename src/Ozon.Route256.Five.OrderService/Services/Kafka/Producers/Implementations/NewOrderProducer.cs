using System.Text;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Ozon.Route256.Five.OrderService.Contracts.KafkaMessages.NewOrder;
using Ozon.Route256.Five.OrderService.Services.Kafka.Settings;

namespace Ozon.Route256.Five.OrderService.Services.Kafka.Producers.Implementations;

public class NewOrderProducer : JsonProducer<string, NewOrderMessage>, IProducer<NewOrderMessage>
{
    private readonly IKafkaBinaryProducer _kafkaBinaryProducer;
    private readonly ProducerSettings _producerSettings;

    public NewOrderProducer(IKafkaBinaryProducer kafkaBinaryProducer, IOptions<KafkaOptions> kafkaOptions)
    {
        _kafkaBinaryProducer = kafkaBinaryProducer;
        _producerSettings = kafkaOptions.Value.ProducerSettings.GetValueOrDefault(ProducerName)
                            ?? throw new InvalidOperationException(
                                $"Не задан конфиг для консьюмера {ProducerName}");
    }

    public Task Send(NewOrderMessage message, CancellationToken cancellationToken)
    {
        var transformedMessage = ToMessage(
            new Message<string, NewOrderMessage>()
            {
                Value = message,
            });
        transformedMessage.Key = Encoding.UTF8.GetBytes(message.OrderId.ToString());
        return _kafkaBinaryProducer.SendMessage(
            _producerSettings.Topic,
            transformedMessage,
            cancellationToken);
    }

    public static string ProducerName => "NewOrderProducer";
}