using Ozon.Route256.Five.OrderService.Contracts.GetForRegions;
using Ozon.Route256.Five.OrderService.Model.OrderAggregate;
using Ozon.Route256.Five.OrderService.Services.Repository.Abstractions;

namespace Ozon.Route256.Five.OrderService.Services.Repository;

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly InMemoryStore _inMemoryStore;

    public InMemoryOrderRepository(InMemoryStore inMemoryStore)
    {
        _inMemoryStore = inMemoryStore;
    }
    
    public Task<IReadOnlyList<OrderAggregate>> GetAllByRegions(
        IReadOnlyList<string>? regions,
        bool? isAscending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
            var query = _inMemoryStore.Orders
            .Select(x => x.Value)
            .Where(x => regions == null || regions.Contains(x.Customer.Address.Region));

        if (isAscending != null)
            query = isAscending.Value
                ? query.OrderBy(x => x.Customer.Address.Region)
                : query.OrderByDescending(x => x.Customer.Address.Region);

        query = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return Task.FromResult((IReadOnlyList<OrderAggregate>)query.ToList())
            .WaitAsync(cancellationToken);
    }

    public Task<OrderAggregate?> GetOrderById(long id, CancellationToken cancellationToken)
        => Task.FromResult(_inMemoryStore.Orders.GetValueOrDefault(id)).WaitAsync(cancellationToken);

    public Task<IReadOnlyList<OrderAggregate>> GetAllForCustomer(
        int customerId,
        DateTime? startFrom,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var result = _inMemoryStore.Orders
            .Select(x => x.Value)
            .Where(
                x =>
                    x.Customer.Id == customerId
                    && (startFrom == null || x.OrderedAt > startFrom.Value))
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        return Task.FromResult((IReadOnlyList<OrderAggregate>)result)
            .WaitAsync(cancellationToken);
    }

    public Task<IReadOnlyList<GetForRegionsResponseItem>> GetForRegions(
        IReadOnlyList<string> regions,
        DateTime startFrom,
        CancellationToken cancellationToken)
    {
        var result = _inMemoryStore.Orders
            .Select(x => x.Value)
            .Where(x => x.OrderedAt > startFrom && regions.Contains(x.Customer.Address.Region))
            .GroupBy(x => x.Customer.Address.Region, x => x)
            .Select(
                x => new GetForRegionsResponseItem(
                    x.Key,
                    x.Count(),
                    x.DistinctBy(y => y.Customer.Id).Count(),
                    x.Sum(y => y.TotalPrice),
                    x.Sum(y => y.TotalWeight)))
            .ToList();
        
        return Task.FromResult((IReadOnlyList<GetForRegionsResponseItem>)result)
            .WaitAsync(cancellationToken);
    }

    public Task Upsert(OrderAggregate value, CancellationToken cancellationToken)
    {
        _inMemoryStore.Orders[value.Id] = value;
        return Task.CompletedTask.WaitAsync(cancellationToken);
    }
}