using System.Collections.Concurrent;
using Bogus;
using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Five.CustomersService.Grpc;
using Ozon.Route256.Five.OrderService.Mappings;
using Ozon.Route256.Five.OrderService.Model;
using Ozon.Route256.Five.OrderService.Model.OrderAggregate;

namespace Ozon.Route256.Five.OrderService.Repository;

public class InMemoryStore
{
    private readonly Customers.CustomersClient _customersClient;
    private readonly IReadOnlyList<string> _regionsList = new[] { "Moscow", "StPetersburg", "Novosibirsk" };
    private readonly ConcurrentDictionary<long, OrderAggregate> _orders = new(2, 10);

    public InMemoryStore(Customers.CustomersClient customersClient)
    {
        _customersClient = customersClient;
    }

    public IReadOnlyList<string> Regions => _regionsList;
    
    public ConcurrentDictionary<long, OrderAggregate> Orders => _orders;

    public async Task FillData()
    {
        var customers = (await _customersClient.GetCustomersAsync(new Empty()))
            .Customers
            .Select(x => new CustomerDto(x.Id, x.DefaultAddress.ToModel()))
            .ToList();

        var goods = new Faker<OrderGood>("ru")
            .CustomInstantiator(
                f => new(
                    f.IndexVariable++,
                    f.Commerce.Product(),
                    f.Random.Int(1, 10),
                    f.Random.Decimal(1, 100000),
                    f.Random.Double(0.1, 200)));

        var orders = new Faker<OrderAggregate>("ru")
            .CustomInstantiator(
                f => new(
                    f.IndexVariable++,
                    f.Random.Enum<OrderState>(),
                    f.Random.ListItem(customers),
                    goods.Generate(f.Random.Int(1, 15)).ToList(),
                    f.Date.Between(new(2022, 1, 1), DateTime.Now),
                    f.Lorem.Word()));

        var generatedOrders = orders.Generate(100);

        foreach (var generatedOrder in generatedOrders)
            _orders[generatedOrder.Id] = generatedOrder;
    }
}