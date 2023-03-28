using FluentAssertions;
using Moq;
using Ozon.Route256.Five.OrderService.Contracts.KafkaMessages.NewOrder;
using Ozon.Route256.Five.OrderService.Features.ProcessGeneratedOrder;
using Ozon.Route256.Five.OrderService.Mappings;
using Ozon.Route256.Five.OrderService.Model;
using Ozon.Route256.Five.OrderService.Model.OrderAggregate;
using Ozon.Route256.Five.OrderService.Services.Domain;
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
        var customerGrpcDto = FakeDataGenerators.CustomerServiceCustomerDtos.First();
        var customerDto = new CustomerDto(customerGrpcDto.Id, customerGrpcDto.DefaultAddress.ToModel());
        
        var fakeOrder = FakeDataGenerators.ModelOrderAggregates.First() with
        {
            OrderState = OrderState.Created,
            OrderedAt = DateTime.UtcNow,
            Customer = new CustomerModel(customerDto.Id, customerGrpcDto.MobileNumber, customerDto.Address)
        };
        var fakeAddress = fakeOrder.Customer.Address;

        var customersClientMock = CustomerServiceMockHelper.WithGetCustomerData(customerGrpcDto);
        
        var orderRepositoryMock = new Mock<IOrderRepository>();
        orderRepositoryMock
            .Setup(x => x.Insert(It.IsAny<OrderAggregate>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var regionRepositoryMock = new Mock<IRegionRepository>();
        regionRepositoryMock.Setup(
                x => x.GetRegionWarehouseAddress(fakeOrder.Customer.Address.Region, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeAddress);

        var producerMock = new ProducerMock<string, NewOrderMessage>();
        var command = new ProcessGeneratedOrderCommand(
            fakeOrder.Id,
            fakeOrder.OrderType,
            customerDto,
            fakeOrder.Goods);

        var handler = new ProcessGeneratedOrderCommandHandler(
            LoggerMock.GetILogger<ProcessGeneratedOrderCommandHandler>().Object,
            orderRepositoryMock.Object,
            regionRepositoryMock.Object,
            producerMock.Object,
            new DateTimeProvider(),
            customersClientMock.Object);
        var result = await handler.Handle(command, default);

        result.Should().BeSuccessful();
        
        orderRepositoryMock.Verify(x => x.Insert(It.IsAny<OrderAggregate>(), It.IsAny<CancellationToken>()));
        producerMock.SentMessages.Should()
            .ContainSingle()
            .Which
            .Should().Be((fakeOrder.Id.ToString(), new NewOrderMessage(fakeOrder.Id)));
    }
    
    /// <summary>
    /// Обработчик, должен сохранить заказ и не отправить в логистику, если он не валиден
    /// </summary>
    [Fact]
    public async Task Handle_ProcessGeneratedOrder_ShouldCreateOrderIfNotValid()
    {
        var customerGrpcDto = FakeDataGenerators.CustomerServiceCustomerDtos.First();
        var customerDto = new CustomerDto(customerGrpcDto.Id, customerGrpcDto.DefaultAddress.ToModel());
        
        var fakeOrder = FakeDataGenerators.ModelOrderAggregates.First() with
        {
            OrderState = OrderState.Created,
            OrderedAt = DateTime.UtcNow,
            Customer = new CustomerModel(
                customerDto.Id,
                customerGrpcDto.MobileNumber,
                customerDto.Address with
                {
                    Longitude = 0,
                    Latitude = 0,
                })
        };
        
        var fakeAddress = fakeOrder.Customer.Address;

        var customersClientMock = CustomerServiceMockHelper.WithGetCustomerData(customerGrpcDto);

        var orderRepositoryMock = new Mock<IOrderRepository>();
        orderRepositoryMock
            .Setup(x => x.Insert(It.IsAny<OrderAggregate>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var regionRepositoryMock = new Mock<IRegionRepository>();
        regionRepositoryMock.Setup(
                x => x.GetRegionWarehouseAddress(fakeOrder.Customer.Address.Region, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeAddress);

        var producerMock = new ProducerMock<string, NewOrderMessage>();
        var command = new ProcessGeneratedOrderCommand(
            fakeOrder.Id,
            fakeOrder.OrderType,
            customerDto,
            fakeOrder.Goods);

        var handler = new ProcessGeneratedOrderCommandHandler(
            LoggerMock.GetILogger<ProcessGeneratedOrderCommandHandler>().Object,
            orderRepositoryMock.Object,
            regionRepositoryMock.Object,
            producerMock.Object,
            new DateTimeProvider(),
            customersClientMock.Object);
        var result = await handler.Handle(command, default);

        result.Should().BeSuccessful();
        
        orderRepositoryMock.Verify(x => x.Insert(It.IsAny<OrderAggregate>(), It.IsAny<CancellationToken>()));
        producerMock.SentMessages.Should().BeEmpty();
    }
}