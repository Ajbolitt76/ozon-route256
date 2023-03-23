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

public static class KafkaServiceCollectionsExtensions
{
    public static IServiceCollection AddKafka(this IServiceCollection sc, IConfiguration configuration)
    {
        sc.AddOptions<KafkaOptions>()
            .Bind(configuration.GetSection("Kafka"))
            .ValidateOnStart()
            .ValidateDataAnnotations();

        sc.AddSingleton<IKafkaBinaryProducer, KafkaBinaryProducer>();

        sc.RegisterDeserializers()
            .AddConsumer<long, PreOrderMessage, PreOrderConsumerHandler>(configuration)
            .AddConsumer<string, OrderEventMessage, OrderUpdateConsumerHandler>(configuration);

        sc.AddProducer<string, NewOrderMessage, NewOrderMessageProducer>(configuration);

        return sc;
    }

    public static IServiceCollection AddProducer<TKey, TMessage, TProducer>(
        this IServiceCollection sc,
        IConfiguration configuration)
        where TProducer : class, IMessageProducer<TKey, TMessage>
    {
        var kafkaOptions = configuration.GetSection("Kafka")
            .Get<KafkaOptions>();

        _ = kafkaOptions?.ProducerSettings.GetValueOrDefault(TProducer.ProducerName)
            ?? throw new InvalidOperationException(
                $"Не задан конфиг для продьюсера {TProducer.ProducerName}");

        sc.AddSingleton<IMessageProducer<TKey, TMessage>, TProducer>();

        return sc;
    }

    public static IServiceCollection AddConsumer<TKey, TMessage, THandler>(
        this IServiceCollection services,
        IConfiguration configuration,
        IDeserializer<TKey>? keyDeserializer = null,
        IDeserializer<TMessage>? valueDeserializer = null) where THandler : class, IKafkaConsumerHandler<TKey, TMessage>
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
                    keyDeserializer ?? sp.GetRequiredService<IDeserializer<TKey>>(),
                    valueDeserializer ?? sp.GetRequiredService<IDeserializer<TMessage>>());
            });
        services.AddScoped<THandler>();
        return services;
    }

    private static IServiceCollection RegisterDeserializers(this IServiceCollection sc)
    {
        sc.AddSingleton(typeof(IDeserializer<>), typeof(KafkaJsonSerializer<>));
        sc.AddSingleton(Deserializers.Int64);
        sc.AddSingleton(Deserializers.Utf8);

        return sc;
    }
}