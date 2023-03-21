using Ozon.Route256.Five.OrderService.Contracts.KafkaMessages.OrderEvents;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Features.UpdateOrderStatus;

namespace Ozon.Route256.Five.OrderService.Services.Kafka.Consumers.Implementations;

public class OrderUpdateConsumer : IKafkaConsumerHandler<string, OrderEventMessage>
{
    private readonly ICommandDispatcher _commandDispatcher;

    public OrderUpdateConsumer(ICommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
    }
    
    public static string ConsumerName => "OrderUpdateConsumer";
    
    public Task Handle(string key, OrderEventMessage message, CancellationToken token)
    {
        return _commandDispatcher.Dispatch(
            new UpdateOrderStatusCommand(message.OrderId, message.NewState, message.ChangedAt),
            token);
    }
}