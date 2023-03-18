using Confluent.Kafka;
using Ozon.Route256.Five.OrderService.Contracts.KafkaMessages.NewOrder;
using Ozon.Route256.Five.OrderService.Contracts.KafkaMessages.OrderEvents;
using Ozon.Route256.Five.OrderService.Contracts.KafkaMessages.PreOrder;
using Ozon.Route256.Five.OrderService.Services.Kafka.Consumers;
using Ozon.Route256.Five.OrderService.Services.Kafka.Consumers.Implementations;
using Ozon.Route256.Five.OrderService.Services.Kafka.Producers;
using Ozon.Route256.Five.OrderService.Services.Kafka.Producers.Implementations;
using Ozon.Route256.Five.OrderService.Services.Kafka.Settings;

namespace Ozon.Route256.Five.OrderService.Services.Kafka;

public static class ServiceCollectionsExtensions
{
    public static IServiceCollection AddKafka(this IServiceCollection sc, IConfiguration configuration)
    {
        sc.AddOptions<KafkaOptions>()
            .Bind(configuration.GetSection("Kafka"))
            .ValidateOnStart()
            .ValidateDataAnnotations();
        
        sc.AddSingleton<IKafkaBinaryProducer, KafkaBinaryProducer>();
        
        sc.AddConsumer<long, PreOrderMessage, PreOrderConsumer>(
            configuration,
            Deserializers.Int64,
            new KafkaJsonSerializer<PreOrderMessage>());
        
        sc.AddConsumer<string, OrderEventMessage, OrderUpdateConsumer>(
            configuration,
            Deserializers.Utf8,
            new KafkaJsonSerializer<OrderEventMessage>());

        sc.AddProducer<NewOrderMessage, NewOrderProducer>(configuration);
        
        return sc;
    }

    public static IServiceCollection AddProducer<TMessage, TProducer>(
        this IServiceCollection sc, 
        IConfiguration configuration)
        where TProducer : class, IProducer<TMessage>
    {
        var kafkaOptions = configuration.GetSection("Kafka")
            .Get<KafkaOptions>();

        _ = kafkaOptions?.ProducerSettings.GetValueOrDefault(TProducer.ProducerName)
                               ?? throw new InvalidOperationException(
                                   $"Не задан конфиг для консьюмера {TProducer.ProducerName}");

        sc.AddSingleton<IProducer<TMessage>, TProducer>();

        return sc;
    }
    
    public static IServiceCollection AddConsumer<TKey, TMessage, THandler>(
        this IServiceCollection services,
        IConfiguration configuration,
        IDeserializer<TKey> keyDeserializer,
        IDeserializer<TMessage> valueDeserializer) where THandler : class, IKafkaConsumerHandler<TKey, TMessage>
    {
        var kafkaOptions = configuration.GetSection("Kafka")
            .Get<KafkaOptions>();

        var consumerSettings = kafkaOptions?.ConsumerSettings.GetValueOrDefault(THandler.ConsumerName)
                               ?? throw new InvalidOperationException(
                                   $"Не задан конфиг для консьюмера {THandler.ConsumerName}");

        if (!consumerSettings.Enabled)
            return services;

        services.AddHostedService<BackgroundKafkaConsumer<TKey, TMessage, THandler>>(
            sp =>
            {
                return new BackgroundKafkaConsumer<TKey, TMessage, THandler>(
                    sp,
                    kafkaOptions,
                    consumerSettings,
                    keyDeserializer,
                    valueDeserializer);
            });
        services.AddScoped<THandler>();
        return services;
    }
}