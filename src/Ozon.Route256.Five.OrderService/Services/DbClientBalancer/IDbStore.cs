namespace Ozon.Route256.Five.OrderService.Services.DbClientBalancer;

public interface IDbStore
{
    int BucketCount { get; }
    
    IReadOnlyCollection<int> BucketList { get; }
    
    Task SetEndpointList(IReadOnlyCollection<DbEndpoint> endpoints);

    Task<DbEndpoint?> GetForBucketAsync(int bucketId, CancellationToken cancellationToken);
}