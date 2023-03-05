namespace Ozon.Route256.Five.OrderService.Contracts.GetOrders;

/// <summary>
/// Ответ для запроса <see cref="GetOrdersRequest"/>
/// </summary>
/// <param name="Orders">Заказы</param>
public record GetOrdersResponse(List<GetOrdersResponseItem> Orders);