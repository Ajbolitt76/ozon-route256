using FluentMigrator.Runner;
using Microsoft.Extensions.Options;
using Ozon.Route256.Five.OrderService.Mappings;
using Ozon.Route256.Five.OrderService.Migrations;
using Ozon.Route256.Five.OrderService.Services.DbClientBalancer;
using Ozon.Route256.Five.Sd.Grpc;

namespace Ozon.Route256.Five.OrderService.Services.Database.Migrator;

public class InServiceShardedMigrator
{
    private readonly SdService.SdServiceClient _client;
    private readonly ShardingOptions _shardingOptions;

    public InServiceShardedMigrator(
        SdService.SdServiceClient client,
        IOptions<ShardingOptions> shardingOptions)
    {
        _client = client;
        _shardingOptions = shardingOptions.Value;
    }

    public async Task Migrate()
    {
        var endpoints = await GetEndpoints();

        foreach (var info in endpoints)
        {
            var connectionString = CreateConnectionString(info);
            var serviceProvider = CreateServices(
                connectionString,
                x =>
                {
                    x.BucketsInShard = info.Buckets
                        .Select(BucketNamingConvention.GetBucketSchema)
                        .ToArray();
                });

            using (var scope = serviceProvider.CreateScope())
            {
                UpdateDatabase(serviceProvider);
            }
        }
    }

    private void UpdateDatabase(IServiceProvider provider)
    {
        var runner = provider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    private IServiceProvider CreateServices(string connectionString, Action<MigratorConfiguration> configure)
    {
        var provider = new ServiceCollection()
            .Configure(configure)
            .AddFluentMigratorCore()
            .ConfigureRunner(
                builder => builder
                    .AddPostgres()
                    .WithGlobalConnectionString(connectionString)
                    .WithMigrationsIn(typeof(InitialMigration).Assembly)
            )
            .BuildServiceProvider(false);
        return provider;
    }

    private string CreateConnectionString(DbEndpoint info)
        => _shardingOptions.GetConnectionString(info.HostPort);

    private async Task<DbEndpoint[]> GetEndpoints()
    {
        // wait for sd
        var token = CancellationToken.None;
        using var stream = _client.DbResources(
            new DbResourcesRequest { ClusterName = "orders-cluster" },
            cancellationToken: token);

        await stream.ResponseStream.MoveNext(CancellationToken.None);
        var response = stream.ResponseStream.Current;

        return response.Replicas
            .Select(
                x => new DbEndpoint(
                    $"{x.Host}:{x.Port}",
                    x.Type.ToModel(),
                    x.Buckets.ToArray()))
            .ToArray();
    }
}