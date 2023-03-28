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
    private readonly IConnectionFactory _connectionFactory;

    public DbOrderRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<OrderAggregate>> GetAllByRegions(
        IReadOnlyList<string>? regions,
        bool? isAscending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var connection = await _connectionFactory.GetConnectionAsync();

        var regionsFilter = regions != null ? "\"Customer\" #>> '{\"Address\",\"Region\"}' = ANY(@Regions)" : "TRUE";
        var ordering = isAscending switch
        {
            true => "ORDER BY \"Customer\" #>> '{\"Address\",\"Region\"}' ASC",
            false => "ORDER BY \"Customer\" #>> '{\"Address\",\"Region\"}' DESC",
            _ => string.Empty
        };

        var query = $"""
            SELECT "Id", "OrderState", "Customer", 
                   "Goods", "OrderedAt", "OrderType", 
                   "TotalPrice", "TotalWeight", "ItemsCount"
            FROM "Order"
            WHERE {regionsFilter}
            {ordering}
            LIMIT @Limit
            OFFSET @Offset
        """;

        var data = await connection.QueryAsync<DbPresentation>(
            query,
            new
            {
                Limit = pageSize,
                Offset = (pageNumber - 1) * pageSize,
                Regions = regions
            });
        return data
            .Select(x => x.ToModel())
            .ToList();
    }

    public async Task<OrderAggregate?> GetOrderById(long id, CancellationToken cancellationToken)
    {
        var connection = await _connectionFactory.GetConnectionAsync();
        var query = $"""
            SELECT "Id", "OrderState", "Customer", 
                   "Goods", "OrderedAt", "OrderType", 
                   "TotalPrice", "TotalWeight", "ItemsCount"
            FROM "Order"
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
        var connection = await _connectionFactory.GetConnectionAsync();

        var dateFilter = startFrom.HasValue ? "\"OrderedAt\" > @StartFrom" : "TRUE";

        var query = $"""
            SELECT "Id", "OrderState", "Customer",
                   "Goods", "OrderedAt", "OrderType", 
                   "TotalPrice", "TotalWeight", "ItemsCount"
            FROM "Order"
            WHERE "Customer" ->> 'Id' = @Id::text AND {dateFilter}
            LIMIT @Limit
            OFFSET @Offset
        """;

        var data = await connection.QueryAsync<DbPresentation>(
            query,
            new
            {
                Id = customerId,
                Limit = pageSize,
                StartFrom = startFrom,
                Offset = (pageNumber - 1) * pageSize
            });

        return data.Select(x => x.ToModel()).ToList();
    }

    public async Task<IReadOnlyList<GetForRegionsResponseItem>> GetForRegions(
        IReadOnlyList<string> regions,
        DateTime startFrom,
        CancellationToken cancellationToken)
    {
        var connection = await _connectionFactory.GetConnectionAsync();

        // language=sql
        var query = """
            WITH preparedOrders as
                     (SELECT "Customer" #>> '{"Address", "Region"}' as "Region",
                             "Customer" ->> 'Id'                    as "CustomerId",
                             "TotalPrice",
                             "TotalWeight",
                             "OrderedAt"
                      FROM "Order")
            SELECT "Region",
                   sum("TotalPrice") as "TotalPrice",
                   sum("TotalWeight") as "TotalWeight",
                   count(distinct "CustomerId") as "CustomersCount",
                   count(*) as "OrdersCount"
            FROM preparedOrders
            WHERE "Region" in @Regions AND "OrderedAt" > @StartFrom
            GROUP BY "Region"
        """;

        var data = await connection.QueryAsync<GetForRegionsResponseItem>(
            query,
            new
            {
                Regions = regions,
                StartFrom = startFrom
            });

        return data.ToList();
    }

    public async Task<bool> UpdateStatus(long id, OrderState state, CancellationToken cancellationToken)
    {
        var connection = await _connectionFactory.GetConnectionAsync();

        var query = """
            UPDATE "Order"
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
        var connection = await _connectionFactory.GetConnectionAsync();

        var query = """
            INSERT INTO "Order"("Id", "OrderState", "Customer", 
                   "Goods", "PhoneNumber", "OrderedAt", "OrderType", 
                   "TotalPrice", "TotalWeight", "ItemsCount")
            VALUES (@Id, @OrderState, @Customer::jsonb, 
                   @Goods::jsonb, @PhoneNumber, @OrderedAt, @OrderType, 
                   @TotalPrice, @TotalWeight, @ItemsCount)
        """;

        await connection.ExecuteAsync(query, DbPresentation.FromModel(value));
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