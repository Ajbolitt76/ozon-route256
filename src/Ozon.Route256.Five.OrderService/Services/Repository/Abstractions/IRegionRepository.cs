using Ozon.Route256.Five.OrderService.Model;

namespace Ozon.Route256.Five.OrderService.Services.Repository.Abstractions;

public interface IRegionRepository
{
    public Task<IReadOnlyList<string>> GetAllRegions(CancellationToken cancellationToken);

    public Task<AddressDto?> GetRegionWarehouseAddress(string region, CancellationToken cancellationToken);
}