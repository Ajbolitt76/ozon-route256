namespace Ozon.Route256.Five.OrderService.Services.DbClientBalancer;

public class DbStore : IDbStore
{
    private DbEndpoint[] _endpoints = Array.Empty<DbEndpoint>();
    private Dictionary<int, DbEndpoint> _bucketEndpointMap = new();

    public IReadOnlyCollection<DbEndpoint> Endpoints => _endpoints;

    public int BucketCount => _bucketEndpointMap.Count;

    public int[] BucketList => _bucketEndpointMap.Keys.ToArray();

    public Task SetEndpointList(IReadOnlyCollection<DbEndpoint> newEndpoints)
    {
        _endpoints = newEndpoints.ToArray();
        _bucketEndpointMap = newEndpoints
            .SelectMany(x => x.Buckets.Select(b => (BucketId: b, Endpoint: x)))
            .ToDictionary(x => x.BucketId, x => x.Endpoint);

        return Task.CompletedTask;
    }

    public Task<DbEndpoint?> GetForBucketAsync(int bucketId, CancellationToken cancellationToken)
        => Task.FromResult(_bucketEndpointMap.GetValueOrDefault(bucketId));
}