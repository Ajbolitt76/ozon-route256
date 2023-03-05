using Ozon.Route256.Five.OrderService.Model;

namespace Ozon.Route256.Five.OrderService.Contracts.GetStatus;

/// <summary>
/// Резултат запроса на статус заказа
/// </summary>
/// <param name="OrderState">Статус заказа</param>
public record GetStatusResponse(OrderState OrderState);