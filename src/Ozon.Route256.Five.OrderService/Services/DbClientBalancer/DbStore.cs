namespace Ozon.Route256.Five.OrderService.Services.DbClientBalancer;

public class DbStore : IDbStore
{
    private DbEndpoint[] _endpoints = Array.Empty<DbEndpoint>();

    private uint _currentIndex;

    public IReadOnlyCollection<DbEndpoint> Endpoints => _endpoints;

    public Task SetEndpointList(IReadOnlyCollection<DbEndpoint> newEndpoints)
    {
        _endpoints = newEndpoints.ToArray();
        return Task.CompletedTask;
    }

    public Task<DbEndpoint> GetNextDbEndpointAsync()
    {
        var endpoints = _endpoints;

        var nextIndex = Interlocked.Increment(ref _currentIndex) - 1;
        
        nextIndex %= (uint)endpoints.Length;

        return Task.FromResult(endpoints[nextIndex]);

    }
}