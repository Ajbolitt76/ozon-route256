using System.Data.Common;
using Microsoft.Extensions.Options;
using Npgsql;
using Ozon.Route256.Five.OrderService.Services.DbClientBalancer;

namespace Ozon.Route256.Five.OrderService.Services.Database;

public class ShardedConnectionFactory : IShardedConnectionFactory
{
    private readonly IDbStore _dbStore;
    private readonly ISharder _sharder;
    private readonly ShardingOptions _shardingOptions;

    public ShardedConnectionFactory(
        IDbStore dbStore,
        ISharder sharder,
        IOptions<ShardingOptions> shardingOptions)
    {
        _dbStore = dbStore;
        _sharder = sharder;
        _shardingOptions = shardingOptions.Value;
    }

    public Task<DbConnection> GetConnectionByKeyAsync<T>(T key, CancellationToken cancellationToken)
        => GetConnectionByBucketIdAsync(_sharder.GetBucketId(key), cancellationToken);

    public async Task<DbConnection> GetConnectionByBucketIdAsync(int bucketId, CancellationToken cancellationToken)
    {
        var connectionString = await GetConnectionString(bucketId, cancellationToken);
        return new BucketDbConnection(new NpgsqlConnection(connectionString), bucketId);
    }

    private async Task<string> GetConnectionString(int bucketId, CancellationToken cancellationToken)
    {
        var endpointInfo = await _dbStore.GetForBucketAsync(bucketId, cancellationToken)
            ?? throw new ArgumentOutOfRangeException(nameof(bucketId), $"Бакет {bucketId} не найден");

        return _shardingOptions.GetConnectionString(endpointInfo.HostPort);
    }
}