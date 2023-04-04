using System.Text.Json;
using Dapper;
using Ozon.Route256.Five.OrderService.Contracts.GetForRegions;
using Ozon.Route256.Five.OrderService.Model;
using Ozon.Route256.Five.OrderService.Model.OrderAggregate;
using Ozon.Route256.Five.OrderService.Services.Database;
using Ozon.Route256.Five.OrderService.Services.Repository.Abstractions;

namespace Ozon.Route256.Five.OrderService.Services.Repository;

public class DbOrderRepository : IOrderRepository
{
    private readonly ISharder _sharder;
    private readonly IRegionRepository _regionRepository;
    private readonly IShardedConnectionFactory _shardedConnectionFactory;

    public DbOrderRepository(
        ISharder sharder,
        IRegionRepository regionRepository,
        IShardedConnectionFactory shardedConnectionFactory)
    {
        _sharder = sharder;
        _regionRepository = regionRepository;
        _shardedConnectionFactory = shardedConnectionFactory;
    }

    /// <summary>
    ///  Получить пагинацию по индексу
    /// </summary>
    /// <param name="bucketData">ИД бакетов + доп параметры для запроса</param>
    /// <param name="sql">
    ///     SQL запроса.
    ///     <br></br>
    ///     Должен состоять из 2 запросов: количество строк удовлетворяющих фильтру и запрос ID с фильтром
    ///     <br></br>
    ///     В запрос пробросятся значени доп параметров для бакета, @Limit, @Offset
    /// </param>
    /// <param name="pageNumber">Номер страницы</param>
    /// <param name="pageSize">Размер страницы</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>Список ID для получения</returns>
    private async Task<List<long>> GetPaginatedIdsToFetch(
        IEnumerable<(int BucketId, IDictionary<string, object> AdditionalParameters)> bucketData,
        string sql,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var dataLeftToFetch = pageSize;
        var offsetLeft = (pageNumber - 1) * pageSize;
        var idsToFetch = new List<long>(pageSize);

        foreach (var (bucketId, additionalParametrs) in bucketData)
        {
            if (dataLeftToFetch <= 0)
                break;

            await using var indexConnection =
                await _shardedConnectionFactory.GetConnectionByBucketIdAsync(bucketId, cancellationToken);

            var parameters = new Dictionary<string, object>()
            {
                ["Limit"] = dataLeftToFetch,
                ["Offset"] = offsetLeft
            };

            foreach (var parameter in additionalParametrs)
                parameters.Add(parameter.Key, parameter.Value);

            var indexData = await indexConnection.QueryMultipleAsync(sql, parameters);
            var countInBucket = await indexData.ReadFirstAsync<int>();
            var ids = (await indexData.ReadAsync<long>()).ToList();

            idsToFetch.AddRange(ids);
            // L = L - ReturnedCount
            dataLeftToFetch = pageSize - idsToFetch.Count;
            // O = O - (Count - ReturnedCount)
            offsetLeft -= (countInBucket - idsToFetch.Count);
        }

        return idsToFetch;
    }

    private List<(int BucketId, List<T> Ids)> GetBucketedIds<T>(IEnumerable<T> ids)
        => ids
            .GroupBy(x => _sharder.GetBucketId(x), x => x)
            .Select(x => (BucketId: x.Key, Ids: x.ToList()))
            .ToList();

    private IOrderedEnumerable<T>? ApplySorting<T, TS>(IEnumerable<T> toSort, bool? isAscending, Func<T, TS> getField)
        => isAscending switch
        {
            true => toSort
                .OrderBy(getField),
            false => toSort
                .OrderByDescending(getField),
            _ => null
        };

    public async Task<IReadOnlyList<OrderAggregate>> GetAllByRegions(
        IReadOnlyList<string>? regions,
        bool? isAscending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        if (regions is null || regions.Count == 0)
            regions = await _regionRepository.GetAllRegions(cancellationToken);

        regions = ApplySorting(regions, isAscending, x => x)?.ToList() ?? regions;

        var idsToFetch = await GetPaginatedIdsToFetch(
            regions.Select(
                x => (_sharder.GetBucketId(x),
                    (IDictionary<string, object>)new Dictionary<string, object>() { ["Region"] = x })),
            """
                SELECT count(*) FROM __bucket__.index_region_order WHERE region = @Region;

                SELECT order_id FROM __bucket__.index_region_order 
                WHERE region = @Region
                ORDER BY order_id
                LIMIT @Limit
                OFFSET @Offset;
            """,
            pageNumber,
            pageSize,
            cancellationToken);

        var orders = await GetByIdsAsync(idsToFetch, cancellationToken);

        return ApplySorting(orders, isAscending, x => x.Customer.Address.Region)
            ?.ThenBy(x => x.Id)
            .ToList() ?? orders;
    }

    public async Task<OrderAggregate?> GetOrderById(long id, CancellationToken cancellationToken)
    {
        await using var connection = await _shardedConnectionFactory.GetConnectionByKeyAsync(id, cancellationToken);
        var query = $"""
            SELECT "Id", "OrderState", "Customer", 
                   "Goods", "PhoneNumber", "OrderedAt", "OrderType", 
                   "TotalPrice", "TotalWeight", "ItemsCount"
            FROM __bucket__."Order"
            WHERE "Id" = @Id
        """;

        var data = await connection.QueryFirstOrDefaultAsync<DbPresentation>(
            query,
            new
            {
                Id = id,
            });

        return data?.ToModel();
    }

    public async Task<IReadOnlyList<OrderAggregate>> GetAllForCustomer(
        int customerId,
        DateTime? startFrom,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        await using var connection =
            await _shardedConnectionFactory.GetConnectionByKeyAsync(customerId, cancellationToken);

        var idsToFetch = await GetPaginatedIdsToFetch(
            new[]
            {
                (_sharder.GetBucketId(customerId),
                    (IDictionary<string, object>)new Dictionary<string, object>() { ["CustomerId"] = customerId })
            },
            """
                SELECT count(*) FROM __bucket__.index_customer_order WHERE customer_id = @CustomerId;

                SELECT order_id FROM __bucket__.index_customer_order 
                WHERE customer_id = @CustomerId;
                ORDER BY order_date
                LIMIT @Limit
                OFFSET @Offset;
            """,
            pageNumber,
            pageSize,
            cancellationToken);

        var orders = await GetByIdsAsync(idsToFetch, cancellationToken);

        return orders;
    }

    public async Task<IReadOnlyList<GetForRegionsResponseItem>> GetForRegions(
        IReadOnlyList<string> regions,
        DateTime startFrom,
        CancellationToken cancellationToken)
    {
        // Это OLAP, так-что тут можно выбирать из JSONB 
        // language=sql
        var query = """
            WITH preparedOrders as
                     (SELECT "Customer" #>> '{"Address", "Region"}' as "Region",
                             "Customer" ->> 'Id'                    as "CustomerId",
                             "TotalPrice",
                             "TotalWeight",
                             "OrderedAt"  
                      FROM __bucket__."Order")
            SELECT "Region",
                   count(*) as "OrdersCount",
                   count(distinct "CustomerId") as "CustomersCount",
                   sum("TotalPrice") as "TotalPrice",
                   sum("TotalWeight") as "TotalWeight"
            FROM preparedOrders
            WHERE "Region" = ANY(@Regions) AND "OrderedAt" > @StartFrom
            GROUP BY "Region"
        """;

        var result = new List<GetForRegionsResponseItem>();

        foreach (var bucketId in _sharder.GetAllBucketIds())
        {
            await using var connection =
                await _shardedConnectionFactory.GetConnectionByBucketIdAsync(bucketId, cancellationToken);

            var data = await connection.QueryAsync<GetForRegionsResponseItem>(
                query,
                new
                {
                    Regions = regions,
                    StartFrom = startFrom
                });
            
            result.AddRange(data);
        }


        return result.GroupBy(x => x.Region)
            .Select(
                g => new GetForRegionsResponseItem(
                    g.Key,
                    g.Select(x => x.OrdersCount).Sum(),
                    g.Select(x => x.CustomersCount).Sum(),
                    g.Select(x => x.TotalPrice).Sum(),
                    g.Select(x => x.TotalWeight).Sum()))
            .ToList();
    }

    public async Task<bool> UpdateStatus(long id, OrderState state, CancellationToken cancellationToken)
    {
        await using var connection = await _shardedConnectionFactory.GetConnectionByKeyAsync(id, cancellationToken);

        var query = """
            UPDATE __bucket__."Order"
            SET "OrderState" = @OrderState
            WHERE "Id" = @Id
        """;

        return await connection.ExecuteAsync(
            query,
            new
            {
                Id = id,
                OrderState = state
            }) == 1;
    }

    public async Task Insert(OrderAggregate value, CancellationToken cancellationToken)
    {
        await using var connection =
            await _shardedConnectionFactory.GetConnectionByKeyAsync(value.Id, cancellationToken);

        var query = """
            INSERT INTO __bucket__."Order"("Id", "OrderState", "Customer", 
                   "Goods", "PhoneNumber", "OrderedAt", "OrderType", 
                   "TotalPrice", "TotalWeight", "ItemsCount")
            VALUES (@Id, @OrderState, @Customer::jsonb, 
                   @Goods::jsonb, @PhoneNumber, @OrderedAt, @OrderType, 
                   @TotalPrice, @TotalWeight, @ItemsCount)
        """;

        await connection.ExecuteAsync(query, DbPresentation.FromModel(value));
        await UpsertIndexes(value, cancellationToken);
    }

    private async Task UpsertIndexes(OrderAggregate value, CancellationToken cancellationToken)
    {
        await using var connectionToRegionIndex = await _shardedConnectionFactory.GetConnectionByKeyAsync(
            value.Customer.Address.Region,
            cancellationToken);

        await using var connectionToCustomerIndex = await _shardedConnectionFactory.GetConnectionByKeyAsync(
            value.Customer.Id,
            cancellationToken);

        var upsertRegionIndex = """
            INSERT INTO __bucket__.index_region_order(order_id, region)
            VALUES (@OrderId, @Region)
            ON CONFLICT (order_id) DO UPDATE SET
                region = @Region
        """;

        var upsertCustomerIndex = """
            INSERT INTO __bucket__.index_customer_order(order_id, customer_id, order_date)
            VALUES (@OrderId, @CustomerId, @OrderedAt)
            ON CONFLICT (order_id) DO UPDATE SET
                customer_id = @CustomerId,
                order_date = @OrderedAt
        """;

        await connectionToRegionIndex.ExecuteAsync(
            upsertRegionIndex,
            new
            {
                OrderId = value.Id,
                Region = value.Customer.Address.Region
            });

        await connectionToCustomerIndex.ExecuteAsync(
            upsertCustomerIndex,
            new
            {
                OrderId = value.Id,
                CustomerId = value.Customer.Id,
                value.OrderedAt
            });
    }

    private async Task<List<OrderAggregate>> GetByIdsAsync(IEnumerable<long> idsToFetch,
        CancellationToken cancellationToken)
    {
        var bucketAndOrderIds = GetBucketedIds(idsToFetch);

        var result = new List<OrderAggregate>(idsToFetch.Count());

        foreach (var (bucketId, ids) in bucketAndOrderIds)
        {
            await using var connection =
                await _shardedConnectionFactory.GetConnectionByBucketIdAsync(bucketId, cancellationToken);

            var query = $"""
                SELECT "Id", "OrderState", "Customer", 
                       "Goods", "PhoneNumber", "OrderedAt", "OrderType", 
                       "TotalPrice", "TotalWeight", "ItemsCount"
                FROM __bucket__."Order"
                WHERE "Id" = ANY(@OrderIds)
            """;

            var data = await connection.QueryAsync<DbPresentation>(
                query,
                new
                {
                    OrderIds = ids
                });

            result.AddRange(data.Select(x => x.ToModel()));
        }

        return result;
    }

    private record DbPresentation(
        long Id,
        OrderState OrderState,
        string Customer,
        string Goods,
        string PhoneNumber,
        DateTime OrderedAt,
        OrderType OrderType,
        decimal TotalPrice,
        double TotalWeight,
        int ItemsCount)
    {
        public OrderAggregate ToModel()
            => new(
                Id,
                OrderState,
                JsonSerializer.Deserialize<CustomerModel>(Customer),
                JsonSerializer.Deserialize<List<OrderGood>>(Goods),
                OrderedAt,
                OrderType);

        public static DbPresentation FromModel(OrderAggregate aggregate)
            => new(
                aggregate.Id,
                aggregate.OrderState,
                JsonSerializer.Serialize(aggregate.Customer),
                JsonSerializer.Serialize(aggregate.Goods),
                aggregate.Customer.PhoneNumber,
                aggregate.OrderedAt,
                aggregate.OrderType,
                aggregate.TotalPrice,
                aggregate.TotalWeight,
                aggregate.ItemsCount);
    };
}