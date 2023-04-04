namespace Ozon.Route256.Five.OrderService.Services.Database.BucketGetters;

public interface IBucketGetter<T>
{
    public int GetShardIndex(T value, int shardCount, int startIndex = 0);
}