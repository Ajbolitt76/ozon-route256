using FluentAssertions;
using Grpc.Core;
using Moq;
using Ozon.Route256.Five.CustomersService.Grpc;
using Ozon.Route256.Five.OrderService.Contracts.GetAllOrdersForCustomer;
using Ozon.Route256.Five.OrderService.Exceptions;
using Ozon.Route256.Five.OrderService.Features.GetOrdersForCustomer;
using Ozon.Route256.Five.OrderService.Mappings;
using Ozon.Route256.Five.OrderService.Model.OrderAggregate;
using Ozon.Route256.Five.OrderService.Services.MicroserviceClients;
using Ozon.Route256.Five.OrderService.Services.Repository.Abstractions;
using Ozon.Route256.Five.OrderService.UnitTests.CommonMocks;
using Ozon.Route256.Five.OrderService.UnitTests.Extensions;
using Ozon.Route256.Five.OrderService.UnitTests.Grpc;

namespace Ozon.Route256.Five.OrderService.UnitTests.Features;

public class GetOrdersForCustomerQueryHandlerTest : BaseUnitTest
{
    /// <summary>
    /// GetAllOrders должен возвращать данные, и делать запрос в cutomerService
    /// </summary>
    [Fact]
    public async Task Handle_GetOrdersForCustomer_ShouldReturnData()
    {
        var customer = FakeDataGenerators.CustomerServiceCustomerDtos.First();

        var orderData = FakeDataGenerators.ModelOrderAggregates.First() with
        {
            Customer = new(customer.Id, customer.DefaultAddress.ToModel())
        };

        var customersMock = CustomerServiceMockHelper.WithGetCustomerData(customer);
        var cachedClient = new CachedCustomersClient(customersMock.Object, PassthroughCache.Object);
        
        var orderRepositoryMock = new Mock<IOrderRepository>();
        orderRepositoryMock.Setup(
                x => x.GetAllForCustomer(
                    It.IsAny<int>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OrderAggregate>() { orderData });

        var handler = new GetOrdersForCustomerQueryHandler(
            orderRepositoryMock.Object, 
            cachedClient);
        var result = await handler.Handle(
            new GetOrdersForCustomerQuery(1, DateTime.Today, 1, 2),
            default);

        result.Should()
            .BeSuccessful()
            .WhichRequiredResult()
            .Orders
            .Should()
            .Equal(
                new[]
                {
                    new GetAllOrdersForCustomerResponseItem(
                        orderData.Id,
                        orderData.ItemsCount,
                        orderData.TotalPrice,
                        orderData.TotalWeight,
                        orderData.OrderType,
                        orderData.OrderedAt,
                        orderData.OrderState,
                        customer.FirstName,
                        orderData.Customer.Address,
                        customer.MobileNumber)
                });
    }

    /// <summary>
    /// GetAllOrders должен возвращать 404, если сервис пользователей не в курсе 
    /// </summary>
    [Fact]
    public async Task Handle_GetOrdersForCustomer_ShouldFail()
    {
        var customersMock = new Mock<Customers.CustomersClient>();
        var orderRepositoryMock = new Mock<IOrderRepository>();
        customersMock.Setup(
                x => x.GetCustomerAsync(
                    It.IsAny<GetCustomerByIdRequest>(),
                    It.IsAny<Metadata?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()))
            .Returns(
                GrpcUtils.CreateAsyncUnaryCall<Customer>(
                    null!,
                    exception: new RpcException(new Status(StatusCode.NotFound, "Not found"))));
        
        var cachedClient = new CachedCustomersClient(customersMock.Object, PassthroughCache.Object);
        
        var handler = new GetOrdersForCustomerQueryHandler(
            orderRepositoryMock.Object,
            cachedClient);
        var result = await handler.Handle(
            new GetOrdersForCustomerQuery(1, DateTime.Today, 1, 2),
            default);

        result.Should()
            .FailWithStrict<NotFoundException>();
    }
}