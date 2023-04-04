namespace Ozon.Route256.Five.OrderService.Services.Database.BucketGetters;

public class StringBucketGetter : BaseBucketGetter<string>
{
    protected override int GetHashCode(string value)
    {
        return (int)Farmhash.Sharp.Farmhash.Hash32(value);
    }
}