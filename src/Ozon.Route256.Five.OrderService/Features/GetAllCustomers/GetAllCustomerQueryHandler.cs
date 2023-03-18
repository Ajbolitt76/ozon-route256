using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Five.CustomersService.Grpc;
using Ozon.Route256.Five.OrderService.Contracts.GetAllCustomers;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;
using Ozon.Route256.Five.OrderService.Mappings;
using Ozon.Route256.Five.OrderService.Services.Redis;

namespace Ozon.Route256.Five.OrderService.Features.GetAllCustomers;

public class GetAllCustomerQueryHandler : IQueryHandler<GetAllCustomerQuery, GetAllCustomersResponse>
{
    private readonly Customers.CustomersClient _customersClient;
    private readonly IRedisCache _cache;

    public GetAllCustomerQueryHandler(
        Customers.CustomersClient customersClient,
        IRedisCache cache)
    {
        _customersClient = customersClient;
        _cache = cache;
    }

    public async Task<HandlerResult<GetAllCustomersResponse>> Handle(GetAllCustomerQuery request,
        CancellationToken token)
    {
        var result = await _cache.GetOrSetAsync(
            () => _customersClient.GetCustomersAsync(new Empty(), cancellationToken: token).ResponseAsync,
            null,
            token);

        var responseItems = result.Customers.Select(
                x => new GetAllCustomersResponseItem(
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.MobileNumber,
                    x.Email,
                    x.DefaultAddress.ToModel()))
            .ToList();

        return new GetAllCustomersResponse(responseItems);
    }
}