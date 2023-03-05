namespace Ozon.Route256.Five.OrderService.Model;

/// <summary>
/// Статус заказа
/// </summary>
public enum OrderState
{
    Created = 0,
    SentToCustomer = 1,
    Delivered = 2,
    Lost = 3,
    Cancelled = 4
}