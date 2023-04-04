namespace Ozon.Route256.Five.OrderService.Services.Database;

public static class BucketNamingConvention
{
    public static string GetBucketSchema(int bucketId) => $"bucket_{bucketId}";
}