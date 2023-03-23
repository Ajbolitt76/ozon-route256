using Confluent.Kafka;
using Ozon.Route256.Five.OrderService.Services.Kafka.Settings;

namespace Ozon.Route256.Five.OrderService.Services.Kafka.Consumers;

public class BackgroundKafkaConsumer<TKey, TMessage, THandler> : BackgroundService
    where THandler : IKafkaConsumerHandler<TKey, TMessage>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConsumerSettings _consumerSettings;
    private readonly IDeserializer<TKey> _keyDeserializer;
    private readonly IDeserializer<TMessage> _messageDeserializer;
    private readonly TimeSpan _timeoutForRetry;
    private readonly ConsumerConfig _config;
    private readonly string _topic;
    private readonly ILogger<BackgroundKafkaConsumer<TKey, TMessage, THandler>> _logger;

    public BackgroundKafkaConsumer(
        IServiceProvider serviceProvider,
        KafkaOptions kafkaSettings,
        ConsumerSettings consumerSettings,
        IDeserializer<TKey> keyDeserializer,
        IDeserializer<TMessage> messageDeserializer)
    {
        _logger = serviceProvider.GetRequiredService<ILogger<BackgroundKafkaConsumer<TKey, TMessage, THandler>>>();
        _serviceProvider = serviceProvider;
        _consumerSettings = consumerSettings;
        _keyDeserializer = keyDeserializer;
        _messageDeserializer = messageDeserializer;
        _timeoutForRetry = TimeSpan.FromSeconds(kafkaSettings.TimeoutForRetryInSecond);

        _config = new ConsumerConfig
        {
            GroupId = kafkaSettings.GroupId,
            BootstrapServers = kafkaSettings.BootstrapServers,
            AutoOffsetReset = consumerSettings.AutoOffsetReset,
            EnableAutoOffsetStore = consumerSettings.PerformSynchronousCommit,
            EnableAutoCommit = consumerSettings.AutoCommit
        };

        if (string.IsNullOrWhiteSpace(consumerSettings.Topic))
            throw new InvalidOperationException("Topic is empty");
        _topic = consumerSettings.Topic;
    }
    

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Consume(stoppingToken);
            }
            catch (Exception e)
            {
                if (e is OperationCanceledException or TaskCanceledException)
                {
                    _logger.LogInformation("Завершаем задачу консьюмера {Topic}", _topic);
                    return;
                }
                _logger.LogError(e, "Error in topic {Topic} during kafka consume", _topic);
                await Task.Delay(_timeoutForRetry, stoppingToken);
            }
        }
    }

    private async Task Consume(CancellationToken token)
    {
        using var consumer = new ConsumerBuilder<TKey, TMessage>(_config)
            .SetValueDeserializer(_messageDeserializer)
            .SetKeyDeserializer(_keyDeserializer)
            .Build();
        
        await Task.Yield();
        
        consumer.Subscribe(_topic);
        _logger.LogInformation("Success subscribe to {Topic}", _topic);
        
        while (!token.IsCancellationRequested)
        {
            var consumed = consumer.Consume(token);
            using var sp = _serviceProvider.CreateScope();
            
            await sp.ServiceProvider.GetRequiredService<THandler>()
                .Handle(consumed.Message.Key, consumed.Message.Value, token);
            
            if(_consumerSettings.PerformSynchronousCommit)
                consumer.Commit();
            else
                consumer.StoreOffset(consumed);
        }
    }

}