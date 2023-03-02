using Ozon.Route256.Five.OrderService.Model;

namespace Ozon.Route256.Five.OrderService.Contracts.GetAllCustomers;

public record GetAllCustomersResponseItem(int Id, AddressDto Address);