using Ozon.Route256.Five.LogisticsSimulator.Grpc;

namespace Ozon.Route256.Five.OrderService.Repository.Abstractions;

public interface IRegionRepository
{
    public Task<IReadOnlyList<string>> GetAllRegions(CancellationToken cancellationToken);
}