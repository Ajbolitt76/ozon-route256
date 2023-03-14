using Ozon.Route256.Five.OrderService.Contracts.GetAllCustomers;
using Ozon.Route256.Five.OrderService.Cqrs;

namespace Ozon.Route256.Five.OrderService.Features.GetAllCustomers;

public record GetAllCustomerQuery() : IQuery<GetAllCustomersResponse>;
