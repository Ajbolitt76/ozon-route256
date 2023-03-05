using Ozon.Route256.Five.OrderService.Model;

namespace Ozon.Route256.Five.OrderService.Contracts.GetAllCustomers;

/// <summary>
/// Представление клиента в <see cref="GetAllCustomersResponse"/>
/// </summary>
/// <param name="Id">Id клиента</param>
/// <param name="Address">Представление адреса</param>
public record GetAllCustomersResponseItem(int Id, AddressDto Address);