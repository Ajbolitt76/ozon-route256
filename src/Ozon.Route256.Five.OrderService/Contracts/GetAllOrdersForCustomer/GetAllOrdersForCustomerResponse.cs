namespace Ozon.Route256.Five.OrderService.Contracts.GetAllOrdersForCustomer;

/// <summary>
/// Результат запроса на список заказов для клиента <see cref="GetAllOrdersForCustomerRequest"/>
/// </summary>
public record GetAllOrdersForCustomerResponse(IReadOnlyList<GetAllOrdersForCustomerResponseItem> Orders);