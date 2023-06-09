using Moq;
using Ozon.Route256.Five.OrderService.Features.UpdateOrderStatus;
using Ozon.Route256.Five.OrderService.Model;
using Ozon.Route256.Five.OrderService.Model.OrderAggregate;
using Ozon.Route256.Five.OrderService.Services.Repository.Abstractions;
using Ozon.Route256.Five.OrderService.UnitTests.CommonMocks;
using Ozon.Route256.Five.OrderService.UnitTests.Extensions;

namespace Ozon.Route256.Five.OrderService.UnitTests.Features;

public class UpdateOrderStatusCommandHandlerTest
{
    /// <summary>
    /// Обработчик, должен обновить только поле Status в аггрегате
    /// </summary>
    [Fact]
    public async Task Handle_UpdateOrder_ShouldOnlyChangeStatus()
    {
        var fakeOrder = FakeDataGenerators.ModelOrderAggregates.First() with { OrderState = OrderState.Created };

        var repositoryMock = new Mock<IOrderRepository>();
        repositoryMock
            .Setup(x => x.GetOrderById(fakeOrder.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeOrder);

        repositoryMock.Setup(
                x => x.UpdateStatus(
                    It.IsAny<long>(),
                    It.IsAny<OrderState>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new UpdateOrderStatusCommandHandler(
            repositoryMock.Object,
            LoggerMock.GetILogger<UpdateOrderStatusCommandHandler>().Object);
        var result = await handler.Handle(
            new UpdateOrderStatusCommand(fakeOrder.Id, OrderState.Delivered, DateTime.Now),
            default);

        result.Should()
            .BeSuccessful();

        repositoryMock.Verify(x => x.UpdateStatus(fakeOrder.Id, OrderState.Delivered, It.IsAny<CancellationToken>()));
    }
}