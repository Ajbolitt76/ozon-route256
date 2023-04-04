namespace Ozon.Route256.Five.OrderService.Services.Database.BucketGetters;

public class LongBucketGetter : BaseBucketGetter<long>
{
    protected override int GetHashCode(long value)
    {
        return (int)Farmhash.Sharp.Farmhash.Hash32(BitConverter.GetBytes(value));
    }
}