namespace Ozon.Route256.Five.OrderService.Services.Database;

public interface ISharder
{
    public int GetBucketId<T>(T key);

    public IReadOnlyCollection<int> GetAllBucketIds();
}