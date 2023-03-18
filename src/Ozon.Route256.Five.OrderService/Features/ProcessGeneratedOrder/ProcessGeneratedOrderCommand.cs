using Ozon.Route256.Five.OrderService.Contracts.KafkaMessages.PreOrder;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Model;
using Ozon.Route256.Five.OrderService.Model.OrderAggregate;

namespace Ozon.Route256.Five.OrderService.Features.ProcessGeneratedOrder;

public record ProcessGeneratedOrderCommand(
        long Id,
        OrderType Source,
        CustomerDto Customer,
        IReadOnlyList<OrderGood> Goods)
    : PreOrderMessage(Id, Source, Customer, Goods), ICommand;