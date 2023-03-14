using FluentAssertions;
using Moq;
using Ozon.Route256.Five.OrderService.Contracts.GetForRegions;
using Ozon.Route256.Five.OrderService.Exceptions;
using Ozon.Route256.Five.OrderService.Features.GetForRegions;
using Ozon.Route256.Five.OrderService.Repository.Abstractions;
using Ozon.Route256.Five.OrderService.UnitTests.Extensions;

namespace Ozon.Route256.Five.OrderService.UnitTests.Features;

public class GetForRegionsQueryHandlerTest
{
    private readonly string[] _knownRegions;
    private readonly Mock<IRegionRepository> _regionRepositoryMock;

    public GetForRegionsQueryHandlerTest()
    {
         _knownRegions = new[] { "Test1", "Test2" };
         _regionRepositoryMock = new Mock<IRegionRepository>();
         _regionRepositoryMock
             .Setup(x => x.GetAllRegions(It.IsAny<CancellationToken>()))
             .ReturnsAsync(_knownRegions);
    }
    
    /// <summary>
    /// GetForRegions должен возвращать данные из репозитория
    /// </summary>
    [Fact]
    public async Task Handle_GetForRegions_ShouldReturnData()
    {
        var expectedResult = new[]
        {
            new GetForRegionsResponseItem(_knownRegions[0], 100, 10, 20m, 200)
        };
        
        var orderRepositoryMock = new Mock<IOrderRepository>();
        orderRepositoryMock.Setup(
                x => x.GetForRegions(
                    It.IsAny<IReadOnlyList<string>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var handler = new GetForRegionsQueryHandler(orderRepositoryMock.Object, _regionRepositoryMock.Object);
        var result = await handler.Handle(
            new GetForRegionsQuery(DateTime.Now, new[] { _knownRegions[0] }),
            default);

        result.Should()
            .BeSuccessful()
            .WhichRequiredResult()
            .RegionStats
            .Should()
            .Equal(expectedResult);
    }
    
    /// <summary>
    /// GetForRegions должен падать, если регион неизвестен
    /// </summary>
    [Fact]
    public async Task Handle_GetForRegions_ShouldFailIfRegionNotFound()
    {
        var orderRepositoryMock = new Mock<IOrderRepository>();

        var handler = new GetForRegionsQueryHandler(orderRepositoryMock.Object, _regionRepositoryMock.Object);
        var result = await handler.Handle(
            new GetForRegionsQuery(DateTime.Now, new[] { _knownRegions[0], "random" }),
            default);

        result.Should()
            .FailWithStrict<DomainException>()
            .Which
            .Message.Should().Contain("random");
    }
}