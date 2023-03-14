namespace Ozon.Route256.Five.OrderService.Repository.Abstractions;

public interface IRegionRepository
{
    public Task<IReadOnlyList<string>> GetAllRegions(CancellationToken cancellationToken);
}