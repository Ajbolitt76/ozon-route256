namespace Ozon.Route256.Five.OrderService.Model;

public record AddressDto(
    string Region, 
    string City, 
    string Street, 
    string Building, 
    string Apartment,
    double Latitude,
    double Longitude);