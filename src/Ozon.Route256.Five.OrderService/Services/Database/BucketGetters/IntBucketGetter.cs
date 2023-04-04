namespace Ozon.Route256.Five.OrderService.Services.Database.BucketGetters;

public class IntBucketGetter : BaseBucketGetter<int>
{
    protected override int GetHashCode(int value)
    {
        return (int)Farmhash.Sharp.Farmhash.Hash32(BitConverter.GetBytes(value));
    }
}