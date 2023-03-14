using Ozon.Route256.Five.OrderService.Contracts.GetOrders;
using Ozon.Route256.Five.OrderService.Cqrs;

namespace Ozon.Route256.Five.OrderService.Features.GetAllOrders;

public record GetAllOrdersQuery(
        IReadOnlyList<string>? Regions, 
        bool? IsAscending, 
        int PageNumber, 
        int PageSize)
    : GetOrdersRequest(Regions, IsAscending, PageNumber, PageSize), IQuery<GetOrdersResponse>;