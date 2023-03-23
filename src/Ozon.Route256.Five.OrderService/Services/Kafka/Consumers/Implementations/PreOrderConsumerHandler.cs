using Ozon.Route256.Five.OrderService.Contracts.KafkaMessages.PreOrder;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Features.ProcessGeneratedOrder;

namespace Ozon.Route256.Five.OrderService.Services.Kafka.Consumers.Implementations;

public class PreOrderConsumerHandler : IKafkaConsumerHandler<long, PreOrderMessage>
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly ILogger<PreOrderConsumerHandler> _logger;

    public PreOrderConsumerHandler(ICommandDispatcher commandDispatcher, ILogger<PreOrderConsumerHandler> logger)
    {
        _commandDispatcher = commandDispatcher;
        _logger = logger;
    }
    
    public static string ConsumerName => "PreOrderConsumer";

    public async Task Handle(long key, PreOrderMessage message, CancellationToken token)
    {
        await _commandDispatcher.Dispatch(
            new ProcessGeneratedOrderCommand(
                message.Id,
                message.Source,
                message.Customer,
                message.Goods),
            token);
        _logger.LogInformation("Обработали заказ {Id}", message.Id);
    }
}