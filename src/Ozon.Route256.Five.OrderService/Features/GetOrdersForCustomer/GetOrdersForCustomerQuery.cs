using Ozon.Route256.Five.OrderService.Contracts.GetAllOrdersForCustomer;
using Ozon.Route256.Five.OrderService.Cqrs;

namespace Ozon.Route256.Five.OrderService.Features.GetOrdersForCustomer;

public record GetOrdersForCustomerQuery(
        int ClientId,
        DateTime? StartFrom,
        int PageNumber,
        int PageSize)
    : GetAllOrdersForCustomerRequest(ClientId, StartFrom, PageNumber, PageSize),
      IQuery<GetAllOrdersForCustomerResponse>;