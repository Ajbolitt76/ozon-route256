using Ozon.Route256.Five.OrderService.Contracts.GetOrderById;
using Ozon.Route256.Five.OrderService.Contracts.GetOrders;
using Ozon.Route256.Five.OrderService.Cqrs;

namespace Ozon.Route256.Five.OrderService.Features.GetOrderById;

public record GetOrderByIdQuery(long Id) : GetOrderByIdRequest(Id), IQuery<GetOrderByIdResponse>;