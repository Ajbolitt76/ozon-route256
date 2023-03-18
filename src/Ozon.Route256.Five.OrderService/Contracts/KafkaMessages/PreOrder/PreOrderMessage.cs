using Ozon.Route256.Five.OrderService.Model;
using Ozon.Route256.Five.OrderService.Model.OrderAggregate;

namespace Ozon.Route256.Five.OrderService.Contracts.KafkaMessages.PreOrder;

public record PreOrderMessage(
    long Id,
    OrderType Source,
    CustomerDto Customer,
    IReadOnlyList<OrderGood> Goods);