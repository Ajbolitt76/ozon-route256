using Ozon.Route256.Five.LogisticsSimulator.Grpc;
using Ozon.Route256.Five.OrderService.Contracts.GetOrderById;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;
using Ozon.Route256.Five.OrderService.Exceptions;
using Ozon.Route256.Five.OrderService.Services.MicroserviceClients;
using Ozon.Route256.Five.OrderService.Services.Redis;
using Ozon.Route256.Five.OrderService.Services.Repository.Abstractions;

namespace Ozon.Route256.Five.OrderService.Features.GetOrderById;

public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, GetOrderByIdResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICachedCustomersClient _cachedCustomersClient;

    public GetOrderByIdQueryHandler(
        IOrderRepository orderRepository,
        ICachedCustomersClient cachedCustomersClient)
    {
        _orderRepository = orderRepository;
        _cachedCustomersClient = cachedCustomersClient;
    }

    public async Task<HandlerResult<GetOrderByIdResponse>> Handle(GetOrderByIdQuery request, CancellationToken token)
    {
        var order = await _orderRepository.GetOrderById(request.Id, token);

        if (order is null)
            return NotFoundException.WithStandardMessage<Order>(request.Id);

        var customer = await _cachedCustomersClient.GetCustomerById(order.Customer.Id, token)
            .ToHandlerResult();

        if (!customer.Success)
            return HandlerResult<GetOrderByIdResponse>.FromError(customer.Error);

        return new GetOrderByIdResponse(
            order.Id,
            order.ItemsCount,
            order.TotalPrice,
            order.TotalWeight,
            order.OrderType,
            order.OrderedAt,
            order.OrderState,
            customer.Value.FirstName,
            order.Customer.Address,
            customer.Value.MobileNumber);
    }
}