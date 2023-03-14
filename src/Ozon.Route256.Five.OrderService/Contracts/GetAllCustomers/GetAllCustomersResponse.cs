namespace Ozon.Route256.Five.OrderService.Contracts.GetAllCustomers;

/// <summary>
/// Результат запроса на список клиентов
/// </summary>
/// <param name="Customers">Список клиентов</param>
public record GetAllCustomersResponse(IReadOnlyCollection<GetAllCustomersResponseItem> Customers);