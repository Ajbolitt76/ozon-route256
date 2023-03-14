using Ozon.Route256.Five.OrderService.Model;
using OrderStateGrpc = Ozon.Route256.Five.OrdersService.Grpc.OrderState;
using OrdersAddressDto = Ozon.Route256.Five.OrdersService.Grpc.AddressDto;

namespace Ozon.Route256.Five.OrderService.Mappings;

public static class OrdersMappings
{
    public static OrderStateGrpc ToOrderServiceDto(this OrderState orderState)
        => orderState switch
        {
            OrderState.Cancelled => OrderStateGrpc.Cancelled,
            OrderState.Created => OrderStateGrpc.Created,
            OrderState.Delivered => OrderStateGrpc.Delivered,
            OrderState.SentToCustomer => OrderStateGrpc.SentToCustomer,
            OrderState.Lost => OrderStateGrpc.Lost,
            _ => throw new ArgumentOutOfRangeException(nameof(orderState), orderState, null)
        };

    public static OrdersAddressDto ToOrderServiceDto(this AddressDto addressDto)
        => new()
        {
            Apartment = addressDto.Apartment,
            Building = addressDto.Building,
            Street = addressDto.Street,
            City = addressDto.City,
            Region = addressDto.Region,
            Latitude = addressDto.Latitude,
            Longitude = addressDto.Longitude
        };
}