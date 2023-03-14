using Ozon.Route256.Five.CustomersService.Grpc;
using Ozon.Route256.Five.LogisticsSimulator.Grpc;
using Ozon.Route256.Five.OrderService.Contracts.GetOrderById;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;
using Ozon.Route256.Five.OrderService.Exceptions;
using Ozon.Route256.Five.OrderService.Exceptions.Grpc;
using Ozon.Route256.Five.OrderService.Repository.Abstractions;

namespace Ozon.Route256.Five.OrderService.Features.GetOrderById;

public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, GetOrderByIdResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly Customers.CustomersClient _customersClient;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository, Customers.CustomersClient customersClient)
    {
        _orderRepository = orderRepository;
        _customersClient = customersClient;
    }
    
    public async Task<HandlerResult<GetOrderByIdResponse>> Handle(GetOrderByIdQuery request, CancellationToken token)
    {
        var order = await _orderRepository.GetOrderById(request.Id, token);

        if (order is null)
            return NotFoundException.WithStandardMessage<Order>(request.Id);
        
        var customer = await _customersClient.GetCustomerAsync(new()
        {
            Id = order.Customer.Id
        }, cancellationToken: token).ToHandlerResult(); 
        
        if(!customer.Success)
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