using Ozon.Route256.Five.LogisticsSimulator.Grpc;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;
using Ozon.Route256.Five.OrderService.Exceptions;
using Ozon.Route256.Five.OrderService.Exceptions.Grpc;
using Ozon.Route256.Five.OrderService.Model;
using Ozon.Route256.Five.OrderService.Repository.Abstractions;

namespace Ozon.Route256.Five.OrderService.Features.CancelOrder;

public class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand>
{
    private readonly LogisticsSimulatorService.LogisticsSimulatorServiceClient _logisticsClient;
    private readonly IOrderRepository _orderRepository;

    public CancelOrderCommandHandler(
        LogisticsSimulatorService.LogisticsSimulatorServiceClient logisticsClient,
        IOrderRepository orderRepository)
    {
        _logisticsClient = logisticsClient;
        _orderRepository = orderRepository;
    }

    public async Task<HandlerResult> Handle(CancelOrderCommand request, CancellationToken token)
    {
        var order = await _orderRepository.GetOrderById(request.Id, token);

        if (order is null)
            return HandlerResult.FromError(NotFoundException.WithStandardMessage<Order>());

        var serviceResult = await _logisticsClient.OrderCancelAsync(
                new() { Id = request.Id },
                cancellationToken: token)
            .ToHandlerResult();

        if (!serviceResult.Success)
            return HandlerResult.FromError(serviceResult.Error);

        if (!serviceResult.Value.Success)
            return HandlerResult.FromError(new DomainException("LOGISTICS_ERROR", serviceResult.Value.Error));

        await _orderRepository.Upsert(order with { OrderState = OrderState.Cancelled }, cancellationToken: token);

        return HandlerResult.Ok;
    }
}