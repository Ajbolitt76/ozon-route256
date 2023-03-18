using Ozon.Route256.Five.CustomersService.Grpc;
using Ozon.Route256.Five.OrderService.Contracts.GetOrders;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;
using Ozon.Route256.Five.OrderService.Exceptions;
using Ozon.Route256.Five.OrderService.Services.Redis;
using Ozon.Route256.Five.OrderService.Services.Repository.Abstractions;

namespace Ozon.Route256.Five.OrderService.Features.GetAllOrders;

public class GetAllOrdersQueryHandler : IQueryHandler<GetAllOrdersQuery, GetOrdersResponse>
{
    private readonly IRegionRepository _regionRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly Customers.CustomersClient _customersClient;
    private readonly IRedisCache _redisCache;

    public GetAllOrdersQueryHandler(
        IRegionRepository regionRepository,
        IOrderRepository orderRepository,
        Customers.CustomersClient customersClient,
        IRedisCache redisCache)
    {
        _regionRepository = regionRepository;
        _orderRepository = orderRepository;
        _customersClient = customersClient;
        _redisCache = redisCache;
    }

    public async Task<HandlerResult<GetOrdersResponse>> Handle(GetAllOrdersQuery request, CancellationToken token)
    {
        if (request.Regions != null)
        {
            var unknownRegions = request.Regions
                .Except(await _regionRepository.GetAllRegions(token))
                .ToList();
        
            if(unknownRegions.Any())
                return HandlerResult<GetOrdersResponse>.FromError(
                    new DomainException($"Unknown regions {string.Join(',', unknownRegions)}"));
        }

        var customerData = (await _redisCache.GetOrSetAsync(
                () => _customersClient.GetCustomersAsync(new(), cancellationToken: token).ResponseAsync,
                null,
                token))
            .Customers
            .ToDictionary(x => x.Id, x => x);

        return new GetOrdersResponse(
            (await _orderRepository.GetAllByRegions(
                request.Regions,
                request.IsAscending,
                request.PageNumber,
                request.PageSize,
                token))
            .Select(
                x => new GetOrdersResponseItem(
                    x.Id,
                    x.ItemsCount,
                    x.TotalPrice,
                    x.TotalWeight,
                    x.OrderType,
                    x.OrderedAt,
                    x.OrderState,
                    customerData.GetValueOrDefault(x.Customer.Id)?.FirstName ?? "",
                    x.Customer.Address,
                    customerData.GetValueOrDefault(x.Customer.Id)?.MobileNumber ?? ""))
            .ToList());
    }
}