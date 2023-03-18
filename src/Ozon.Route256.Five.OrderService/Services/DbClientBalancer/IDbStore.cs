namespace Ozon.Route256.Five.OrderService.Services.DbClientBalancer;

public interface IDbStore
{
    Task SetEndpointList(IReadOnlyCollection<DbEndpoint> endpoints);

    Task<DbEndpoint> GetNextDbEndpointAsync();
}