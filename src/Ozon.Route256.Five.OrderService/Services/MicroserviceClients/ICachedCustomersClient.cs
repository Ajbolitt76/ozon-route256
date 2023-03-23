using Ozon.Route256.Five.CustomersService.Grpc;

namespace Ozon.Route256.Five.OrderService.Services.MicroserviceClients;

public interface ICachedCustomersClient
{
    Task<Customer> GetCustomerById(int id, CancellationToken token);
    Task<GetCustomersResponse> GetAllCustomers(CancellationToken token);
}