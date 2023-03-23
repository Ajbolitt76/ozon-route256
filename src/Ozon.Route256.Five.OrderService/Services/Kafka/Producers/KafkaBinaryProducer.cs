using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Ozon.Route256.Five.OrderService.Services.Kafka.Settings;

namespace Ozon.Route256.Five.OrderService.Services.Kafka.Producers;

public class KafkaBinaryProducer : IKafkaBinaryProducer, IDisposable
{
    private readonly IProducer<byte[], byte[]> _producer;
    private readonly ILogger<KafkaBinaryProducer> _logger;

    public KafkaBinaryProducer(ILogger<KafkaBinaryProducer> logger, IOptions<KafkaOptions> kafkaOptions)
    {
        _logger = logger;
        var config = new ProducerConfig
        {
            BootstrapServers = kafkaOptions.Value.BootstrapServers,
            Acks = Acks.Leader,
            EnableIdempotence = false,
        };

        _producer = new ProducerBuilder<byte[], byte[]>(config)
            .Build();
    }

    public async Task SendMessage(
        string topic,
        Message<byte[], byte[]> message,
        CancellationToken token)
    {
        try
        {
            await _producer.ProduceAsync(topic, message, token);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in {Topic} producer", topic);
            throw;
        }
    }
    
    public void Dispose()
    {
        _producer.Flush();
        _producer.Dispose();
    }
}