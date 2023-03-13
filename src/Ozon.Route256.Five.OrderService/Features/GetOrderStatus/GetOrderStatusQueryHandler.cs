using Ozon.Route256.Five.LogisticsSimulator.Grpc;
using Ozon.Route256.Five.OrderService.Contracts.GetStatus;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;
using Ozon.Route256.Five.OrderService.Exceptions;
using Ozon.Route256.Five.OrderService.Repository.Abstractions;

namespace Ozon.Route256.Five.OrderService.Features.GetOrderStatus;

public class GetOrderStatusQueryHandler : IQueryHandler<GetOrderStatusQuery, GetStatusResponse>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderStatusQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public async Task<HandlerResult<GetStatusResponse>> Handle(GetOrderStatusQuery request, CancellationToken token)
    {
        var order = await _orderRepository.GetOrderById(request.Id, token);

        if (order is null)
            return HandlerResult<GetStatusResponse>.FromError(NotFoundException.WithStandardMessage<Order>());

        return new GetStatusResponse(order.Id, order.OrderState);
    }
}