using Ozon.Route256.Five.OrderService.Services.Database.BucketGetters;
using Ozon.Route256.Five.OrderService.Services.Database.Migrator;

namespace Ozon.Route256.Five.OrderService.Services.Database;

public static class DataBaseServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection sc, IConfiguration configuration)
    {
        sc.Configure<ShardingOptions>(configuration.GetSection("Sharding"));

        sc.AddScoped<IShardedConnectionFactory, ShardedConnectionFactory>();
        sc.AddScoped<InServiceShardedMigrator>();
        sc.AddScoped<ISharder, Sharder>();
        
        // Добавить IBucketGetter
        sc.Scan(x =>
            x.FromAssembliesOf(typeof(BaseBucketGetter<>))
                .AddClasses(c => c.AssignableTo(typeof(IBucketGetter<>)))
                .AsImplementedInterfaces()
                .WithSingletonLifetime());
        
        return sc;
    }
}