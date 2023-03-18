using Ozon.Route256.Five.OrderService.Contracts.KafkaMessages.NewOrder;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;
using Ozon.Route256.Five.OrderService.Exceptions;
using Ozon.Route256.Five.OrderService.Model;
using Ozon.Route256.Five.OrderService.Model.Geolocation;
using Ozon.Route256.Five.OrderService.Model.OrderAggregate;
using Ozon.Route256.Five.OrderService.Services.Domain;
using Ozon.Route256.Five.OrderService.Services.Kafka.Producers;
using Ozon.Route256.Five.OrderService.Services.Repository.Abstractions;

namespace Ozon.Route256.Five.OrderService.Features.ProcessGeneratedOrder;

public class ProcessGeneratedOrderCommandHandler : ICommandHandler<ProcessGeneratedOrderCommand>
{
    private readonly ILogger<ProcessGeneratedOrderCommandHandler> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly IRegionRepository _regionRepository;
    private readonly IProducer<NewOrderMessage> _newOrderProducer;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ProcessGeneratedOrderCommandHandler(
        ILogger<ProcessGeneratedOrderCommandHandler> logger,
        IOrderRepository orderRepository,
        IRegionRepository regionRepository, 
        IProducer<NewOrderMessage> newOrderProducer,
        IDateTimeProvider dateTimeProvider)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _regionRepository = regionRepository;
        _newOrderProducer = newOrderProducer;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<HandlerResult> Handle(ProcessGeneratedOrderCommand request, CancellationToken token)
    {
        var order = new OrderAggregate(
            request.Id,
            OrderState.Created,
            request.Customer,
            request.Goods.ToList(),
            _dateTimeProvider.UtcNow,
            request.Source
        );

        await _orderRepository.Upsert(order, token);
        
        var warehouseAddress = await _regionRepository.GetRegionWarehouse(request.Customer.Address.Region, token);
        if (warehouseAddress is null || !WarehouseInCloseDistance(request.Customer.Address, warehouseAddress))
        {
            _logger.LogInformation("Пропущен невалидный заказа {OrderId}", order.Id);
            return HandlerResult.Ok;
        }

        await _newOrderProducer.Send(new NewOrderMessage(order.Id), token);

        return HandlerResult.Ok;
    }

    private bool WarehouseInCloseDistance(AddressDto customer, AddressDto warehouse)
    {
        var point1 = new GeoPoint(customer.Longitude, customer.Latitude);
        var point2 = new GeoPoint(warehouse.Longitude, warehouse.Latitude);

        return point1.DistanceTo(point2) < 5000;
    }
}