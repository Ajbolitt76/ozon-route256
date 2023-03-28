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
        var updateResult =
            await _orderRepository.UpdateStatus(request.OrderId, request.NewState, token).ToHandlerResult();

        if (updateResult is { Success: false })
        {
            _logger.LogError("Обновление заказа не удалось: заказ {Id}", updateResult.Error);
            return HandlerResult.FromError(updateResult.Error);
        }

        if (updateResult is { Value: false })
        {
            _logger.LogWarning("Обновление заказа не удалось: заказ {Id} не найден", request.OrderId);
            return HandlerResult.FromError(
                NotFoundException.WithStandardMessage<OrderAggregate>(request.OrderId.ToString()));
        }

        _logger.LogWarning("Стаутс заказа {Id} обновлен на {NewStatus}", request.OrderId, request.NewState);

        return HandlerResult.Ok;
    }
}