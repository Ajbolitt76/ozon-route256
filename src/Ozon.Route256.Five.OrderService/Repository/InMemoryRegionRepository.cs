using Ozon.Route256.Five.OrderService.Repository.Abstractions;

namespace Ozon.Route256.Five.OrderService.Repository;

public class InMemoryRegionRepository : IRegionRepository
{
    private readonly InMemoryStore _inMemoryStore;

    public InMemoryRegionRepository(InMemoryStore inMemoryStore)
    {
        _inMemoryStore = inMemoryStore;
    }
    
    public Task<IReadOnlyList<string>> GetAllRegions(CancellationToken cancellationToken)
        => Task.FromResult(_inMemoryStore.Regions).WaitAsync(cancellationToken);
}