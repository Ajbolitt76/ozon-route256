using Ozon.Route256.Five.OrderService.Contracts.GetStatus;
using Ozon.Route256.Five.OrderService.Cqrs;

namespace Ozon.Route256.Five.OrderService.Features.GetOrderStatus;

public record GetOrderStatusQuery(long Id) : GetStatusRequest(Id), IQuery<GetStatusResponse>;