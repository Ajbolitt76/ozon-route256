using Ozon.Route256.Five.OrderService.Services.Database.BucketGetters;
using Ozon.Route256.Five.OrderService.Services.DbClientBalancer;

namespace Ozon.Route256.Five.OrderService.Services.Database;

public class Sharder : ISharder
{
    private IServiceProvider _serviceProvider;
    private readonly IDbStore _dbStore;

    public Sharder(IServiceProvider serviceProvider, IDbStore dbStore)
    {
        _serviceProvider = serviceProvider;
        _dbStore = dbStore;
    }
    
    public int GetBucketId<T>(T key)
    {
        var bucketGetter = _serviceProvider.GetService<IBucketGetter<T>>()
                           ?? throw new ArgumentOutOfRangeException(
                               nameof(T), 
                               $"Для типа ${typeof(T).Name} не был найден ${nameof(IBucketGetter<T>)}");

        return bucketGetter.GetShardIndex(key, _dbStore.BucketCount);
    }

    public int[] GetAllBucketIds()
        => _dbStore.BucketList;
}