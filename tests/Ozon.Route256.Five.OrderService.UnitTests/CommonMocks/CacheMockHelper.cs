using Moq;
using Ozon.Route256.Five.OrderService.Services.Redis;

namespace Ozon.Route256.Five.OrderService.UnitTests.CommonMocks;

public static class CacheMockHelper
{
    public static Mock<IRedisCache> CreatePassthroughCache()
    {
        var mock = new Mock<IRedisCache>();
        mock.Setup(
            x => x.GetOrSetAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<It.IsAnyType>>>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<CancellationToken>()))
            .Returns(new InvocationFunc(
                inv => ((Delegate)inv.Arguments[1]).DynamicInvoke()));
        
        mock.Setup(
                x => x.GetOrSetAsync(
                    It.IsAny<Func<Task<It.IsAnyType>>>(),
                    It.IsAny<TimeSpan?>(),
                    It.IsAny<CancellationToken>()))
            .Returns(new InvocationFunc(
                inv => ((Delegate)inv.Arguments[0]).DynamicInvoke()));
        
        return mock;
    }
}