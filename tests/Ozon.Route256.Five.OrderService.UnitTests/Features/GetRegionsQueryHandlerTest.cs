using FluentAssertions;
using Moq;
using Ozon.Route256.Five.OrderService.Features.GetRegions;
using Ozon.Route256.Five.OrderService.Services.Repository.Abstractions;
using Ozon.Route256.Five.OrderService.UnitTests.Extensions;

namespace Ozon.Route256.Five.OrderService.UnitTests.Features;

public class GetRegionsQueryHandlerTest
{
    [Fact]
    public async Task Handle_GetRegions_ShouldReturnData()
    {
        var expected = new[] { "Test1", "Test2" };
        var regionsRepoMock = new Mock<IRegionRepository>();
        regionsRepoMock.Setup(x => x.GetAllRegions(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetRegionsQueryHandler(regionsRepoMock.Object);
        var result = await handler.Handle(new GetRegionsQuery(), default);

        result.Should()
            .BeSuccessful()
            .WhichRequiredResult()
            .Regions.Should().Equal(expected);
    }
}