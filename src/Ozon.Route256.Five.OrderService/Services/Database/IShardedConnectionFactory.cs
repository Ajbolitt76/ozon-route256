using System.Data.Common;

namespace Ozon.Route256.Five.OrderService.Services.Database;

public interface IShardedConnectionFactory
{
    public Task<DbConnection> GetConnectionByKeyAsync<T>(T key, CancellationToken cancellationToken);

    public Task<DbConnection> GetConnectionByBucketIdAsync(int bucketId, CancellationToken cancellationToken);
}