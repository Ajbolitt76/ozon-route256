using Ozon.Route256.Five.OrderService.Model;
using Ozon.Route256.Five.OrderService.Services.Repository.Abstractions;

namespace Ozon.Route256.Five.OrderService.Services.Repository;

public class InMemoryRegionRepository : IRegionRepository
{
    private readonly InMemoryStore _inMemoryStore;

    public InMemoryRegionRepository(InMemoryStore inMemoryStore)
    {
        _inMemoryStore = inMemoryStore;
    }

    public Task<IReadOnlyList<string>> GetAllRegions(CancellationToken cancellationToken)
        => Task.FromResult((IReadOnlyList<string>)_inMemoryStore.Regions.Select(x => x.Name).ToList());

    public Task<AddressDto?> GetRegionWarehouseAddress(string region, CancellationToken cancellationToken)
        => Task.FromResult(_inMemoryStore.Regions.FirstOrDefault(x => x.Name == region)?.Address);
}