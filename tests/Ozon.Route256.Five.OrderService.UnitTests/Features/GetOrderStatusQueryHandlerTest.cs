using FluentAssertions;
using Moq;
using Ozon.Route256.Five.OrderService.Contracts.GetStatus;
using Ozon.Route256.Five.OrderService.Features.GetOrderStatus;
using Ozon.Route256.Five.OrderService.Services.Repository.Abstractions;
using Ozon.Route256.Five.OrderService.UnitTests.CommonMocks;
using Ozon.Route256.Five.OrderService.UnitTests.Extensions;

namespace Ozon.Route256.Five.OrderService.UnitTests.Features;

public class GetOrderStatusQueryHandlerTest
{
    /// <summary>
    /// GetAllOrders должен возвращать данные, и делать запрос в cutomerService
    /// </summary>
    [Fact]
    public async Task Handle_GetAllOrders_ShouldReturnData()
    {
        var orderData = FakeDataGenerators.ModelOrderAggregates.First();
        
        var orderRepositoryMock = new Mock<IOrderRepository>();
        orderRepositoryMock.Setup(
                x => x.GetOrderById(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderData);

        var handler = new GetOrderStatusQueryHandler(orderRepositoryMock.Object);
        var result = await handler.Handle(new(orderData.Id), default);

        result.Should()
            .BeSuccessful()
            .WhichRequiredResult()
            .Should()
            .Be(new GetStatusResponse(orderData.Id, orderData.OrderState));
    }
}