using Ozon.Route256.Five.OrderService.Model;

namespace Ozon.Route256.Five.OrderService.Contracts.GetOrders;

/// <summary>
/// Элементы запроса <see cref="GetOrdersRequest"/>
/// Представление заказа
/// </summary>
public record GetOrdersResponseItem(
    int Id,
    uint ItemsCount,
    double TotalPrice,
    double TotalWeight,
    string OrderType,
    DateTime OrderedAt,
    OrderState OrderState,
    string ClientName,
    AddressDto ShippingAddress,
    string Phone);