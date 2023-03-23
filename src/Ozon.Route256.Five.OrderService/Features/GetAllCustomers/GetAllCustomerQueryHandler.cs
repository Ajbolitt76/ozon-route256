using Ozon.Route256.Five.OrderService.Contracts.GetAllCustomers;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;
using Ozon.Route256.Five.OrderService.Mappings;
using Ozon.Route256.Five.OrderService.Services.MicroserviceClients;

namespace Ozon.Route256.Five.OrderService.Features.GetAllCustomers;

public class GetAllCustomerQueryHandler : IQueryHandler<GetAllCustomerQuery, GetAllCustomersResponse>
{
    private readonly ICachedCustomersClient _cachedCustomersClient;

    public GetAllCustomerQueryHandler(
        ICachedCustomersClient cachedCustomersClient)
    {
        _cachedCustomersClient = cachedCustomersClient;
    }

    public async Task<HandlerResult<GetAllCustomersResponse>> Handle(
        GetAllCustomerQuery request,
        CancellationToken token)
    {
        var result = await _cachedCustomersClient.GetAllCustomers(token);

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