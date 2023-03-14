using Ozon.Route256.Five.OrderService.Contracts.GetForRegions;
using Ozon.Route256.Five.OrderService.Model.OrderAggregate;

namespace Ozon.Route256.Five.OrderService.Repository.Abstractions;

public interface IOrderRepository
{
    Task<IReadOnlyList<OrderAggregate>> GetAllByRegions(
        IReadOnlyList<string>? regions,
        bool? isAscending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<OrderAggregate?> GetOrderById(long id, CancellationToken cancellationToken);
    
    Task<IReadOnlyList<OrderAggregate>> GetAllForCustomer(
        int customerId,
        DateTime? startFrom,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);


    Task<IReadOnlyList<GetForRegionsResponseItem>> GetForRegions(
        IReadOnlyList<string> regions, 
        DateTime startFrom,
        CancellationToken cancellationToken);

    Task Upsert(OrderAggregate value, CancellationToken cancellationToken);
}