using Ozon.Route256.Five.CustomersService.Grpc;
using Ozon.Route256.Five.OrderService.Model;

namespace Ozon.Route256.Five.OrderService.Services.Repository;

public class InMemoryStore
{
    private readonly Customers.CustomersClient _customersClient;

    private readonly IReadOnlyList<Region> _regionsList = new Region[]
    {
        new("Moscow", new("Moscow", "Moscow", "", "7", "", 55.62362601674, 37.4290940251)),
        new("StPetersburg", new("StPetersburg", "StPetersburg", "ул. Софийская", "118", "", 59.814295, 30.478282)),
        new("Novosibirsk", new("Novosibirsk", "Novosibirsk", "3307 км", "16", "", 55.004917, 82.555067)),
    };
    
    public InMemoryStore(Customers.CustomersClient customersClient)
    {
        _customersClient = customersClient;
    }

    public IReadOnlyList<Region> Regions => _regionsList;
}