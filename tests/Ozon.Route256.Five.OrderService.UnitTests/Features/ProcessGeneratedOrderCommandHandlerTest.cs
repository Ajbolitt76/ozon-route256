using FluentAssertions;
using Moq;
using Ozon.Route256.Five.OrderService.Contracts.KafkaMessages.NewOrder;
using Ozon.Route256.Five.OrderService.Features.ProcessGeneratedOrder;
using Ozon.Route256.Five.OrderService.Model;
using Ozon.Route256.Five.OrderService.Model.OrderAggregate;
using Ozon.Route256.Five.OrderService.Services.Repository.Abstractions;
using Ozon.Route256.Five.OrderService.UnitTests.CommonMocks;
using Ozon.Route256.Five.OrderService.UnitTests.Extensions;

namespace Ozon.Route256.Five.OrderService.UnitTests.Features;

public class ProcessGeneratedOrderCommandHandlerTest : BaseUnitTest
{
    /// <summary>
    /// Обработчик, должен сохранить заказ и отправить в логистику, если он валиден
    /// </summary>
    [Fact]
    public async Task Handle_ProcessGeneratedOrder_ShouldCreateOrderAndSendIfValid()
    {
        var fakeAddress = FakeDataGenerators.ModelAddressDtos.First();
        var fakeOrder = FakeDataGenerators.ModelOrderAggregates.First();

        fakeOrder = fakeOrder with
        {
            OrderState = OrderState.Created,
            OrderedAt = DateTime.UtcNow,
            Customer = fakeOrder.Customer with
            {
                Address = fakeAddress
            }
        };

        var orderRepositoryMock = new Mock<IOrderRepository>();
        orderRepositoryMock
            .Setup(x => x.Upsert(It.IsAny<OrderAggregate>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var regionRepositoryMock = new Mock<IRegionRepository>();
        regionRepositoryMock.Setup(
                x => x.GetRegionWarehouse(fakeOrder.Customer.Address.Region, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeAddress);

        var producerMock = new ProducerMock<NewOrderMessage>();
        var command = new ProcessGeneratedOrderCommand(
            fakeOrder.Id,
            fakeOrder.OrderType,
            fakeOrder.Customer,
            fakeOrder.Goods);

        var handler = new ProcessGeneratedOrderCommandHandler(
            LoggerMock.GetILogger<ProcessGeneratedOrderCommandHandler>().Object,
            orderRepositoryMock.Object,
            regionRepositoryMock.Object,
            producerMock.Object,
            DateTimeProvider.Object);
        var result = await handler.Handle(command, default);

        result.Should().BeSuccessful();
        
        orderRepositoryMock.Verify(x => x.Upsert(It.IsAny<OrderAggregate>(), It.IsAny<CancellationToken>()));
        producerMock.SentMessages.Should()
            .ContainSingle()
            .Which
            .Should().Be(new NewOrderMessage(fakeOrder.Id));
    }
    
        /// <summary>
    /// Обработчик, должен сохранить заказ и не отправить в логистику, если он не валиден
    /// </summary>
    [Fact]
    public async Task Handle_ProcessGeneratedOrder_ShouldCreateOrderIfNotValid()
    {
        var fakeAddress = FakeDataGenerators.ModelAddressDtos.First();
        var fakeOrder = FakeDataGenerators.ModelOrderAggregates.First();

        fakeOrder = fakeOrder with
        {
            OrderState = OrderState.Created,
            OrderedAt = DateTime.UtcNow,
            Customer = fakeOrder.Customer with
            {
                Address = fakeAddress with
                {
                    Longitude = 0,
                    Latitude = 0,
                }
            }
        };

        var orderRepositoryMock = new Mock<IOrderRepository>();
        orderRepositoryMock
            .Setup(x => x.Upsert(It.IsAny<OrderAggregate>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var regionRepositoryMock = new Mock<IRegionRepository>();
        regionRepositoryMock.Setup(
                x => x.GetRegionWarehouse(fakeOrder.Customer.Address.Region, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeAddress);

        var producerMock = new ProducerMock<NewOrderMessage>();
        var command = new ProcessGeneratedOrderCommand(
            fakeOrder.Id,
            fakeOrder.OrderType,
            fakeOrder.Customer,
            fakeOrder.Goods);

        var handler = new ProcessGeneratedOrderCommandHandler(
            LoggerMock.GetILogger<ProcessGeneratedOrderCommandHandler>().Object,
            orderRepositoryMock.Object,
            regionRepositoryMock.Object,
            producerMock.Object,
            DateTimeProvider.Object);
        var result = await handler.Handle(command, default);

        result.Should().BeSuccessful();
        
        orderRepositoryMock.Verify(x => x.Upsert(It.IsAny<OrderAggregate>(), It.IsAny<CancellationToken>()));
        producerMock.SentMessages.Should().BeEmpty();
    }
}