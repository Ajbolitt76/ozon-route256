namespace Ozon.Route256.Five.OrderService.Model;

public enum OrderState
{
    Created,
    SentToCustomer,
    Delivered,
    Lost,
    Cancelled
}