using System.Text;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Ozon.Route256.Five.OrderService.Contracts.KafkaMessages.NewOrder;
using Ozon.Route256.Five.OrderService.Services.Kafka.Settings;

namespace Ozon.Route256.Five.OrderService.Services.Kafka.Producers.Implementations;

public class NewOrderMessageProducer : BaseKeyedProducer<NewOrderMessageProducer, string, NewOrderMessage>,
                                       IMessageProducer<string, NewOrderMessage>
{
    public NewOrderMessageProducer(IKafkaBinaryProducer kafkaBinaryProducer, IOptions<KafkaOptions> kafkaOptions) :
        base(kafkaBinaryProducer, kafkaOptions)
    {
    }

    protected override byte[] SerializeKey(string key)
        => Encoding.UTF8.GetBytes(key);

    public static string ProducerName => "NewOrderProducer";
}