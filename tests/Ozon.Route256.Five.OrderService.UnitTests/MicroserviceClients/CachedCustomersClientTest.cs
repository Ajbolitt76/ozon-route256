using Moq;
using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ozon.Route256.Five.CustomersService.Grpc;
using Ozon.Route256.Five.OrderService.Consts;
using Ozon.Route256.Five.OrderService.Services.MicroserviceClients;
using Ozon.Route256.Five.OrderService.UnitTests.CommonMocks;

namespace Ozon.Route256.Five.OrderService.UnitTests.MicroserviceClients;

public class CachedCustomersClientTest : BaseUnitTest
{
    /// <summary>
    /// Метод GetById должен кэшировать
    /// </summary>
    [Fact]
    public async Task CachedCustomersClient_GetById_MustCacheById()
    {
        var customer = FakeDataGenerators.CustomerServiceCustomerDtos.First();
        var customersMock = CustomerServiceMockHelper.WithGetCustomerData(customer);

        var service = new CachedCustomersClient(customersMock.Object, PassthroughCache.Object);
        var result = await service.GetCustomerById(customer.Id, default);

        result.Should().Be(customer);
        
        PassthroughCache.Verify(x => x.GetOrSetAsync(
            It.Is<string>(key => key == customer.Id.ToString()),
            It.IsAny<Func<Task<Customer>>>(),
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()
            ), Times.Once);
        
        customersMock.Verify(x => x.GetCustomerAsync(
            It.Is<GetCustomerByIdRequest>(
                req => req.Id == customer.Id),
            It.IsAny<Metadata>(),
            It.IsAny<DateTime?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
    
    /// <summary>
    /// Метод GetAll должен кэшировать
    /// </summary>
    [Fact]
    public async Task CachedCustomersClient_GetAll_MustCache()
    {
        var customersMock = CustomerServiceMockHelper.WithGetCustomersData(Array.Empty<Customer>());

        var service = new CachedCustomersClient(customersMock.Object, PassthroughCache.Object);
        var result = await service.GetAllCustomers(default);

        result.Customers.Should().Equal(Array.Empty<Customer>());
        
        PassthroughCache.Verify(x => x.GetOrSetAsync(
            It.Is<string>(key => key == CacheKeys.AllCustomersCacheKey),
            It.IsAny<Func<Task<GetCustomersResponse>>>(),
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        
        customersMock.Verify(x => x.GetCustomersAsync(
            It.IsAny<Empty>(),
            It.IsAny<Metadata>(),
            It.IsAny<DateTime?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}