using Ozon.Route256.Five.CustomersService.Grpc;
using Ozon.Route256.Five.OrderService.Consts;
using Ozon.Route256.Five.OrderService.Services.Redis;

namespace Ozon.Route256.Five.OrderService.Services.MicroserviceClients;

/// <summary>
/// Кэшированный клиент микросервиса покупателей
/// </summary>
public class CachedCustomersClient : ICachedCustomersClient
{
    private readonly Customers.CustomersClient _customersClient;
    private readonly IRedisCache _redisCache;

    public CachedCustomersClient(Customers.CustomersClient customersClient, IRedisCache redisCache)
    {
        _customersClient = customersClient;
        _redisCache = redisCache;
    }

    public Task<Customer> GetCustomerById(int id, CancellationToken token)
        => _redisCache.GetOrSetAsync(
            id.ToString(),
            async () => await _customersClient.GetCustomerAsync(
                new()
                {
                    Id = id
                },
                cancellationToken: token),
            null,
            token);

    public Task<GetCustomersResponse> GetAllCustomers(CancellationToken token)
        => _redisCache.GetOrSetAsync(
            CacheKeys.AllCustomersCacheKey,
            async () => await _customersClient.GetCustomersAsync(new(), cancellationToken: token),
            null,
            token);
}