using Bogus;
using FluentAssertions;
using Moq;
using Ozon.Route256.Five.OrderService.Contracts.GetOrders;
using Ozon.Route256.Five.OrderService.Exceptions;
using Ozon.Route256.Five.OrderService.Features.GetAllOrders;
using Ozon.Route256.Five.OrderService.Mappings;
using Ozon.Route256.Five.OrderService.Model.OrderAggregate;
using Ozon.Route256.Five.OrderService.Services.Repository.Abstractions;
using Ozon.Route256.Five.OrderService.UnitTests.CommonMocks;
using Ozon.Route256.Five.OrderService.UnitTests.Extensions;

namespace Ozon.Route256.Five.OrderService.UnitTests.Features;

public class GetAllOrdersQueryHandlerTest : BaseUnitTest
{
    private readonly Faker _faker = new Faker();
    private readonly string[] _knownRegions;
    private readonly Mock<IRegionRepository> _regionRepositoryMock;

    public GetAllOrdersQueryHandlerTest()
    {
        _knownRegions = new[] { "Test1", "Test2" };
        _regionRepositoryMock = new Mock<IRegionRepository>();
        _regionRepositoryMock
            .Setup(x => x.GetAllRegions(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_knownRegions);
    }
    
    /// <summary>
    /// GetAllOrders должен возвращать данные, и делать запрос в cutomerService
    /// </summary>
    [Fact]
    public async Task Handle_GetAllOrders_ShouldReturnData()
    {
        var customer = FakeDataGenerators.CustomerServiceCustomerDtos.First();

        var orderData = FakeDataGenerators.ModelOrderAggregates.First() with
        {
            Customer = new(customer.Id, customer.DefaultAddress.ToModel())
        };

        var customersMock = CustomerServiceMockHelper.WithGetCustomersData(new[] { customer });
        var orderRepositoryMock = new Mock<IOrderRepository>();
        orderRepositoryMock.Setup(
                x => x.GetAllByRegions(
                    It.IsAny<IReadOnlyList<string>?>(),
                    It.IsAny<bool?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OrderAggregate>()
            {
                orderData
            });

        var handler = new GetAllOrdersQueryHandler(
            _regionRepositoryMock.Object, 
            orderRepositoryMock.Object,
            customersMock.Object,
            PassthroughCache.Object);

        var result = await handler.Handle(
            new GetAllOrdersQuery(new List<string>{ _knownRegions[0] }, true, 0, 0),
            default);

        result.Should()
            .BeSuccessful()
            .WhichRequiredResult()
            .Orders
            .Should()
            .Equal(new []{
                new GetOrdersResponseItem(
                    orderData.Id,
                    orderData.ItemsCount,
                    orderData.TotalPrice,
                    orderData.TotalWeight,
                    orderData.OrderType,
                    orderData.OrderedAt,
                    orderData.OrderState,
                    customer.FirstName,
                    orderData.Customer.Address,
                    customer.MobileNumber)});
    }
    
    /// <summary>
    /// GetAllOrders должен падать если регион неизвестен
    /// </summary>
    [Fact]
    public async Task Handle_GetAllOrders_ShouldFailIfRegionUnknown()
    {
        var customer = FakeDataGenerators.CustomerServiceCustomerDtos.First();
        
        var customersMock = CustomerServiceMockHelper.WithGetCustomersData(new[] { customer });
        var orderRepositoryMock = new Mock<IOrderRepository>();

        var handler = new GetAllOrdersQueryHandler(
            _regionRepositoryMock.Object, 
            orderRepositoryMock.Object,
            customersMock.Object,
            PassthroughCache.Object);

        var result = await handler.Handle(
            new GetAllOrdersQuery(new List<string>
            {
                "kek"
            }, true, 0, 0),
            default);

        result.Should().FailWithStrict<DomainException>()
            .Which
            .Message.Should().Contain("kek");
    }
}