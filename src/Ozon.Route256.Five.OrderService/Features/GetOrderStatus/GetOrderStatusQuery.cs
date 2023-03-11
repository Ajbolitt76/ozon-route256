using Ozon.Route256.Five.OrderService.Contracts.GetOrders;
using Ozon.Route256.Five.OrderService.Contracts.GetStatus;
using Ozon.Route256.Five.OrderService.Cqrs;

namespace Ozon.Route256.Five.OrderService.Features.GetOrderStatus;

public record GetOrderStatusQuery(int Id) : GetStatusRequest(Id), IQuery<GetStatusResponse>;