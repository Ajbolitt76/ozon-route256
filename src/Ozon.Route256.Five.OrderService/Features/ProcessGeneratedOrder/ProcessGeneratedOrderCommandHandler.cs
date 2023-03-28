using Ozon.Route256.Five.CustomersService.Grpc;
using Ozon.Route256.Five.OrderService.Contracts.KafkaMessages.NewOrder;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;
using Ozon.Route256.Five.OrderService.Model;
using Ozon.Route256.Five.OrderService.Model.Geolocation;
using Ozon.Route256.Five.OrderService.Model.OrderAggregate;
using Ozon.Route256.Five.OrderService.Services.Domain;
using Ozon.Route256.Five.OrderService.Services.Kafka.Producers;
using Ozon.Route256.Five.OrderService.Services.Repository.Abstractions;

namespace Ozon.Route256.Five.OrderService.Features.ProcessGeneratedOrder;

public class ProcessGeneratedOrderCommandHandler : ICommandHandler<ProcessGeneratedOrderCommand>
{
    private const int MaximalDistanceFromWareHouse = 5000 * 1000;
    private readonly ILogger<ProcessGeneratedOrderCommandHandler> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly IRegionRepository _regionRepository;
    private readonly IMessageProducer<string, NewOrderMessage> _newOrderMessageProducer;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly Customers.CustomersClient _customersClient;

    public ProcessGeneratedOrderCommandHandler(
        ILogger<ProcessGeneratedOrderCommandHandler> logger,
        IOrderRepository orderRepository,
        IRegionRepository regionRepository, 
        IMessageProducer<string, NewOrderMessage> newOrderMessageProducer,
        IDateTimeProvider dateTimeProvider,
        Customers.CustomersClient customersClient)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _regionRepository = regionRepository;
        _newOrderMessageProducer = newOrderMessageProducer;
        _dateTimeProvider = dateTimeProvider;
        _customersClient = customersClient;
    }

    public async Task<HandlerResult> Handle(ProcessGeneratedOrderCommand request, CancellationToken token)
    {
        var customerFromService = await _customersClient.GetCustomerAsync(new GetCustomerByIdRequest()
        {
            Id = request.Customer.Id
        }, cancellationToken: token);

        var customerModel = new CustomerModel(
            request.Customer.Id,
            customerFromService.MobileNumber,
            request.Customer.Address
        );

        var order = new OrderAggregate(
            request.Id,
            OrderState.Created,
            customerModel,
            request.Goods.ToList(),
            _dateTimeProvider.UtcNow,
            request.Source
        );

        await _orderRepository.Insert(order, token);
        
        var warehouseAddress = await _regionRepository.GetRegionWarehouseAddress(request.Customer.Address.Region, token);
        if (warehouseAddress is null || !WarehouseInCloseDistance(request.Customer.Address, warehouseAddress))
        {
            _logger.LogInformation("Пропущен невалидный заказа {OrderId}", order.Id);
            return HandlerResult.Ok;
        }

        await _newOrderMessageProducer.Send(order.Id.ToString(), new NewOrderMessage(order.Id), token);

        return HandlerResult.Ok;
    }

    private bool WarehouseInCloseDistance(AddressDto customer, AddressDto warehouse)
    {
        var point1 = new GeoPoint(customer.Longitude, customer.Latitude);
        var point2 = new GeoPoint(warehouse.Longitude, warehouse.Latitude);

        return point1.DistanceTo(point2) < MaximalDistanceFromWareHouse;
    }
}