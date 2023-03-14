using Bogus;
using FluentAssertions;
using Moq;
using Ozon.Route256.Five.OrderService.Contracts.GetOrderById;
using Ozon.Route256.Five.OrderService.Exceptions;
using Ozon.Route256.Five.OrderService.Features.GetOrderById;
using Ozon.Route256.Five.OrderService.Mappings;
using Ozon.Route256.Five.OrderService.Model.OrderAggregate;
using Ozon.Route256.Five.OrderService.Repository.Abstractions;
using Ozon.Route256.Five.OrderService.UnitTests.CommonMocks;
using Ozon.Route256.Five.OrderService.UnitTests.Extensions;

namespace Ozon.Route256.Five.OrderService.UnitTests.Features;

public class GetOrderByIdQueryHandlerTest
{
    private readonly Faker _faker = new Faker();

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

        var customersMock = CustomerServiceMockHelper.WithGetCustomerData(customer);
        var orderRepositoryMock = new Mock<IOrderRepository>();
        orderRepositoryMock.Setup(
                x => x.GetOrderById(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderData);

        var handler = new GetOrderByIdQueryHandler(orderRepositoryMock.Object, customersMock.Object);
        var result = await handler.Handle(
            new GetOrderByIdQuery(12),
            default);

        result.Should()
            .BeSuccessful()
            .WhichRequiredResult()
            .Should()
            .Be(
                new GetOrderByIdResponse(
                    orderData.Id,
                    orderData.ItemsCount,
                    orderData.TotalPrice,
                    orderData.TotalWeight,
                    orderData.OrderType,
                    orderData.OrderedAt,
                    orderData.OrderState,
                    customer.FirstName,
                    orderData.Customer.Address,
                    customer.MobileNumber));
    }
    
    /// <summary>
    /// GetAllOrders должен падать, если заказ не найден
    /// </summary>
    [Fact]
    public async Task Handle_GetAllOrders_ShouldFailIfNotFound()
    {
        var customer = FakeDataGenerators.CustomerServiceCustomerDtos.First();

        var customersMock = CustomerServiceMockHelper.WithGetCustomerData(customer);
        var orderRepositoryMock = new Mock<IOrderRepository>();
        orderRepositoryMock.Setup(
                x => x.GetOrderById(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderAggregate?)null);

        var handler = new GetOrderByIdQueryHandler(orderRepositoryMock.Object, customersMock.Object);
        var result = await handler.Handle(
            new GetOrderByIdQuery(12),
            default);

        result.Should()
            .FailWithStrict<NotFoundException>();
    }
}