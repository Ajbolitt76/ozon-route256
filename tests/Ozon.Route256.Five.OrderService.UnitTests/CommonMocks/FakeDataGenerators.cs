using Bogus;
using Ozon.Route256.Five.OrderService.Model;
using Ozon.Route256.Five.OrderService.Model.OrderAggregate;

namespace Ozon.Route256.Five.OrderService.UnitTests.CommonMocks;

public static class FakeDataGenerators
{
    private static readonly string[] _regions = new[] { "Moscow", "StPetersburg", "Novosibirsk" };

    public static IEnumerable<AddressDto> ModelAddressDtos { get; }
        = new Faker<AddressDto>("ru")
            .CustomInstantiator(
                f => new AddressDto(
                    f.Random.ArrayElement(_regions),
                    f.Address.City(),
                    f.Address.StreetName(),
                    f.Address.BuildingNumber(),
                    f.Address.SecondaryAddress(),
                    f.Address.Latitude(),
                    f.Address.Longitude()))
            .GenerateForever();

    public static IEnumerable<CustomersService.Grpc.Address> CustomerServiceAddressDtos { get; }
        = new Faker<CustomersService.Grpc.Address>("ru")
            .CustomInstantiator(
                f => new CustomersService.Grpc.Address
                {
                    City = f.Address.City(),
                    Apartment = f.Address.SecondaryAddress(),
                    Building = f.Address.BuildingNumber(),
                    Region = f.Random.ArrayElement(_regions),
                    Street = f.Address.StreetName(),
                    Latitude = f.Address.Latitude(),
                    Longitude = f.Address.Longitude(),
                })
            .GenerateForever();

    public static IEnumerable<CustomersService.Grpc.Customer> CustomerServiceCustomerDtos { get; }
        = new Faker<CustomersService.Grpc.Customer>("ru")
            .CustomInstantiator(
                f => new CustomersService.Grpc.Customer()
                {
                    Id = f.Random.Number(Int32.MaxValue),
                    Addresses = { CustomerServiceAddressDtos.Take(f.Random.Number(1,3)) },
                    DefaultAddress = CustomerServiceAddressDtos.First(),
                    Email = f.Internet.Email(),
                    FirstName = f.Name.FirstName(),
                    LastName = f.Name.LastName(),
                    MobileNumber = f.Phone.PhoneNumber()
                })
            .GenerateForever();
    
    public static IEnumerable<CustomerDto> ModelCustomerDtos { get; }
        = new Faker<CustomerDto>("ru")
            .CustomInstantiator(
                f => new CustomerDto(f.Random.Number(Int32.MaxValue), ModelAddressDtos.First()))
            .GenerateForever();

    public static IEnumerable<OrderGood> ModelOrderGoods { get; }
        = new Faker<OrderGood>("ru")
            .CustomInstantiator(
                f => new(
                    f.Random.Number(Int32.MaxValue),
                    f.Commerce.Product(),
                    f.Random.Int(1, 10),
                    f.Random.Decimal(1, 100000),
                    f.Random.Double(0.1, 200)))
            .GenerateForever();

    public static IEnumerable<OrderAggregate> ModelOrderAggregates { get; }
        = new Faker<OrderAggregate>("ru")
            .CustomInstantiator(
                f => new(
                    f.Random.Number(Int32.MaxValue),
                    f.Random.Enum<OrderState>(),
                    ModelCustomerDtos.First(),
                    ModelOrderGoods.Take(f.Random.Int(1, 15)).ToList(),
                    f.Date.Between(new(2022, 1, 1), DateTime.Now),
                    f.Random.Enum<OrderType>()))
            .GenerateForever();
}