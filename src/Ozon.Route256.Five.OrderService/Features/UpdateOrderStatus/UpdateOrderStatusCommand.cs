using Ozon.Route256.Five.OrderService.Contracts.KafkaMessages.OrderEvents;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Model;

namespace Ozon.Route256.Five.OrderService.Features.UpdateOrderStatus;

public record UpdateOrderStatusCommand(
        long OrderId,
        OrderState NewState,
        DateTime ChangedAt) 
    : OrderEventMessage(OrderId, NewState, ChangedAt), ICommand;