using Ozon.Route256.Five.CustomersService.Grpc;

namespace Ozon.Route256.Five.OrderService.Mappings;

public static class CustomerServiceMappings
{
    public static Model.AddressDto MapToModel(this Address grpcDto)
        => new(
            grpcDto.Region,
            grpcDto.City,
            grpcDto.Street,
            grpcDto.Building,
            grpcDto.Apartment,
            grpcDto.Latitude,
            grpcDto.Longitude);
}