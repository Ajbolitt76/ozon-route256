using Ozon.Route256.Five.OrderService.Model;

namespace Ozon.Route256.Five.OrderService.Contracts.GetAllOrdersForClient;

/// <summary>
/// Элемент списка заказов клиента в <see cref="GetAllOrdersForClientResponse"/>
/// </summary>
public record GetAllOrdersForClientResponseItem(
    int Id,
    uint ItemsCount,
    decimal TotalPrice,
    double TotalWeight,
    string OrderType,
    DateTime OrderedAt,
    OrderState OrderState,
    string ClientName,
    AddressDto ShippingAddress,
    string Phone);