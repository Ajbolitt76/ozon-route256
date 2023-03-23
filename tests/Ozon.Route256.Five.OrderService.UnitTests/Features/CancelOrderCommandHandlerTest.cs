using Moq;
using Grpc.Core;
using Ozon.Route256.Five.LogisticsSimulator.Grpc;
using Ozon.Route256.Five.OrderService.Exceptions;
using Ozon.Route256.Five.OrderService.Features.CancelOrder;
using Ozon.Route256.Five.OrderService.Services.Repository.Abstractions;
using Ozon.Route256.Five.OrderService.UnitTests.CommonMocks;
using Ozon.Route256.Five.OrderService.UnitTests.Extensions;
using Ozon.Route256.Five.OrderService.UnitTests.Grpc;

namespace Ozon.Route256.Five.OrderService.UnitTests.Features;

/// <summary>
/// Тест для <see cref="CancelOrderCommandHandler"/>
/// </summary>
public class CancelOrderCommandHandlerTest
{
    /// <summary>
    /// Обработчик должен отменить в БД, и в сервисе логистике
    /// </summary>
    [Fact]
    public async Task Handle_CancelOrder_ShouldCancelInDbAndLogistics()
    {
        var logisticsMock = new Mock<LogisticsSimulatorService.LogisticsSimulatorServiceClient>();
        logisticsMock.Setup(
                x => x.OrderCancelAsync(
                    It.IsAny<Order>(),
                    It.IsAny<Metadata?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()))
            .Returns(
                GrpcUtils.CreateAsyncUnaryCall(
                    new CancelResult()
                    {
                        Success = true,
                        Error = ""
                    }));

        var orderRepositoryMock = new Mock<IOrderRepository>();
        orderRepositoryMock.Setup(x => x.GetOrderById(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((long id, CancellationToken _) => FakeDataGenerators.ModelOrderAggregates.First() with { Id = id });

        var handler = new CancelOrderCommandHandler(logisticsMock.Object, orderRepositoryMock.Object);
        
        var result = await handler.Handle(new CancelOrderCommand(2), default);

        result.Should().BeSuccessful();
        
        logisticsMock.Verify(
            x => x.OrderCancelAsync(
                It.IsAny<Order>(),
                It.IsAny<Metadata?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    /// <summary>
    /// Обработчик должен вернуть ошибку, если заказ не найден
    /// </summary>
    [Fact]
    public async Task Handle_CancelOrder_ShouldNotCallLogisticsIfNotFound()
    {
        var logisticsMock = new Mock<LogisticsSimulatorService.LogisticsSimulatorServiceClient>();
        logisticsMock.Setup(
                x => x.OrderCancelAsync(
                    It.IsAny<Order>(),
                    It.IsAny<Metadata?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()))
            .Returns(
                GrpcUtils.CreateAsyncUnaryCall(
                    new CancelResult()
                    {
                        Success = true,
                        Error = ""
                    }));

        var orderRepositoryMock = new Mock<IOrderRepository>();
        orderRepositoryMock.Setup(x => x.GetOrderById(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => null);

        var handler = new CancelOrderCommandHandler(logisticsMock.Object, orderRepositoryMock.Object);
        
        var result = await handler.Handle(new CancelOrderCommand(2), default);

        result.Should().FailWith<NotFoundException>();
        
        logisticsMock.Verify(
            x => x.OrderCancelAsync(
                It.IsAny<Order>(),
                It.IsAny<Metadata?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}