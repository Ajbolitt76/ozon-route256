using Ozon.Route256.Five.OrderService.Model;

namespace Ozon.Route256.Five.OrderService.Contracts.GetOrderById;

public record GetOrderByIdResponse(
    int Id,
    int ItemsCount,
    decimal TotalPrice,
    double TotalWeight,
    string OrderType,
    DateTime OrderedAt,
    OrderState OrderState,
    string ClientName,
    AddressDto ShippingAddress,
    string Phone);