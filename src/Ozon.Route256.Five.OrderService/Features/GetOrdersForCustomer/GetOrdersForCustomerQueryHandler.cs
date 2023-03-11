using Ozon.Route256.Five.CustomersService.Grpc;
using Ozon.Route256.Five.OrderService.Contracts.GetAllOrdersForCustomer;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;
using Ozon.Route256.Five.OrderService.Exceptions.Grpc;
using Ozon.Route256.Five.OrderService.Repository.Abstractions;

namespace Ozon.Route256.Five.OrderService.Features.GetOrdersForCustomer;

public class GetOrdersForCustomerQueryHandler
    : IQueryHandler<GetOrdersForCustomerQuery, GetAllOrdersForCustomerResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly Customers.CustomersClient _customersClient;

    public GetOrdersForCustomerQueryHandler(IOrderRepository orderRepository, Customers.CustomersClient customersClient)
    {
        _orderRepository = orderRepository;
        _customersClient = customersClient;
    }

    public async Task<HandlerResult<GetAllOrdersForCustomerResponse>> Handle(
        GetOrdersForCustomerQuery request,
        CancellationToken token)
    {
        var clientInfoResult = await _customersClient.GetCustomerAsync(
            new GetCustomerByIdRequest()
            {
                Id = request.ClientId
            }).ToHandlerResult();

        if (!clientInfoResult.Success)
            return HandlerResult<GetAllOrdersForCustomerResponse>.FromError(clientInfoResult.Error);

        var clientInfo = clientInfoResult.Value;

        var items = (await _orderRepository.GetAllForCustomer(
                request.ClientId,
                request.StartFrom,
                request.PageNumber,
                request.PageSize,
                token))
            .Select(
                x => new GetAllOrdersForCustomerResponseItem(
                    x.Id,
                    x.ItemsCount,
                    x.TotalPrice,
                    x.TotalWeight,
                    x.OrderType,
                    x.OrderedAt,
                    x.OrderState,
                    clientInfo.FirstName,
                    x.Customer.Address,
                    clientInfo.MobileNumber))
            .ToList();

        return new GetAllOrdersForCustomerResponse(items);
    }
}