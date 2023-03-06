namespace Ozon.Route256.Five.OrderService.DbClientBalancer;

public class DbStore : IDbStore
{
    private DbEndpoint[] _endpoints = Array.Empty<DbEndpoint>();

    private int _currentIndex = -1;

    public IReadOnlyCollection<DbEndpoint> Endpoints => _endpoints;

    public Task SetEndpointList(IReadOnlyCollection<DbEndpoint> newEndpoints)
    {
        _endpoints = newEndpoints.ToArray();
        return Task.CompletedTask;
    }

    public Task<DbEndpoint> GetNextDbEndpointAsync()
    {
        var endpoints = _endpoints;

        var nextIndex = Interlocked.Increment(ref _currentIndex);

        nextIndex %= endpoints.Length;

        return Task.FromResult(endpoints[nextIndex]);

    }
}