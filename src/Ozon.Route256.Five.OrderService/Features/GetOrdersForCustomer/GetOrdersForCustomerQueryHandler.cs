using Ozon.Route256.Five.CustomersService.Grpc;
using Ozon.Route256.Five.OrderService.Contracts.GetAllOrdersForCustomer;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;
using Ozon.Route256.Five.OrderService.Exceptions;
using Ozon.Route256.Five.OrderService.Exceptions.Grpc;
using Ozon.Route256.Five.OrderService.Services.Redis;
using Ozon.Route256.Five.OrderService.Services.Repository.Abstractions;

namespace Ozon.Route256.Five.OrderService.Features.GetOrdersForCustomer;

public class GetOrdersForCustomerQueryHandler
    : IQueryHandler<GetOrdersForCustomerQuery, GetAllOrdersForCustomerResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly Customers.CustomersClient _customersClient;
    private readonly IRedisCache _redisCache;

    public GetOrdersForCustomerQueryHandler(
        IOrderRepository orderRepository,
        Customers.CustomersClient customersClient,
        IRedisCache redisCache)
    {
        _orderRepository = orderRepository;
        _customersClient = customersClient;
        _redisCache = redisCache;
    }

    public async Task<HandlerResult<GetAllOrdersForCustomerResponse>> Handle(
        GetOrdersForCustomerQuery request,
        CancellationToken token)
    {
        var clientInfoResult = await _redisCache.GetOrSetAsync(
            request.ClientId.ToString(),
            () => _customersClient.GetCustomerAsync(
                new GetCustomerByIdRequest()
                {
                    Id = request.ClientId
                }).ResponseAsync,
            null,
            cancellationToken: token).ToHandlerResult();

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