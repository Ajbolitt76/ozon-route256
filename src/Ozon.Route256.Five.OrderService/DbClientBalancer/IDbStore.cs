namespace Ozon.Route256.Five.OrderService.DbClientBalancer;

public interface IDbStore
{
    Task SetEndpointList(IReadOnlyCollection<DbEndpoint> endpoints);

    Task<DbEndpoint> GetNextDbEndpointAsync();
}