using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;
using Ozon.Route256.Five.OrderService.Exceptions;
using Ozon.Route256.Five.OrderService.Model.OrderAggregate;
using Ozon.Route256.Five.OrderService.Services.Repository.Abstractions;

namespace Ozon.Route256.Five.OrderService.Features.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler : ICommandHandler<UpdateOrderStatusCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;

    public UpdateOrderStatusCommandHandler(
        IOrderRepository orderRepository,
        ILogger<UpdateOrderStatusCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }
    
    public async Task<HandlerResult> Handle(UpdateOrderStatusCommand request, CancellationToken token)
    {
        var order = await _orderRepository.GetOrderById(request.OrderId, token);

        if (order is null)
        {
            _logger.LogWarning("Обновление заказа не удалось: заказ {Id} не найден", request.OrderId);
            return HandlerResult.FromError(NotFoundException.WithStandardMessage<OrderAggregate>(request.OrderId.ToString()));
        }

        var upResult = await _orderRepository.Upsert(order with { OrderState = request.NewState }, token).ToHandlerResult();
        _logger.LogWarning("Стаутс заказа {Id} обновлен на {NewStatus}", request.OrderId, request.NewState);
        
        return upResult;
    }
}