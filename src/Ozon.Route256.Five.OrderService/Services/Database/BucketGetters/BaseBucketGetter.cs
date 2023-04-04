namespace Ozon.Route256.Five.OrderService.Services.Database.BucketGetters;

public abstract class BaseBucketGetter<T> : IBucketGetter<T>
{
    public int GetShardIndex(T value, int shardCount, int startIndex = 0)
    {
        if (shardCount <= 0)
            throw new ArgumentException("Количество шардов должно быть больше 0", nameof(shardCount));
        return startIndex + Math.Abs(GetHashCode(value)) % shardCount;
    }

    protected abstract int GetHashCode(T value);
}