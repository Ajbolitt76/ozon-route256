using Ozon.Route256.Five.OrderService.Model;
using Ozon.Route256.Five.OrderService.Model.OrderAggregate;

namespace Ozon.Route256.Five.OrderService.Contracts.GetAllOrdersForCustomer;

/// <summary>
/// Элемент списка заказов клиента в <see cref="GetAllOrdersForCustomerResponse"/>
/// </summary>
public record GetAllOrdersForCustomerResponseItem(
    long Id,
    int ItemsCount,
    decimal TotalPrice,
    double TotalWeight,
    OrderType OrderType,
    DateTime OrderedAt,
    OrderState OrderState,
    string ClientName,
    AddressDto ShippingAddress,
    string Phone);