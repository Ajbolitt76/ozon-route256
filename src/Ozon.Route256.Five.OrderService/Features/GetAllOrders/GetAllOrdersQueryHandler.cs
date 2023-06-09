using Ozon.Route256.Five.OrderService.Contracts.GetOrders;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;
using Ozon.Route256.Five.OrderService.Exceptions;
using Ozon.Route256.Five.OrderService.Services.MicroserviceClients;
using Ozon.Route256.Five.OrderService.Services.Repository.Abstractions;

namespace Ozon.Route256.Five.OrderService.Features.GetAllOrders;

public class GetAllOrdersQueryHandler : IQueryHandler<GetAllOrdersQuery, GetOrdersResponse>
{
    private readonly IRegionRepository _regionRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICachedCustomersClient _cachedCustomersClient;

    public GetAllOrdersQueryHandler(
        IRegionRepository regionRepository,
        IOrderRepository orderRepository,
        ICachedCustomersClient cachedCustomersClient)
    {
        _regionRepository = regionRepository;
        _orderRepository = orderRepository;
        _cachedCustomersClient = cachedCustomersClient;
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

        var customerData = (await _cachedCustomersClient.GetAllCustomers(token))
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