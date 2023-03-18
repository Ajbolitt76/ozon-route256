using Moq;
using Ozon.Route256.Five.OrderService.Services.Domain;
using Ozon.Route256.Five.OrderService.Services.Redis;
using Ozon.Route256.Five.OrderService.UnitTests.CommonMocks;

namespace Ozon.Route256.Five.OrderService.UnitTests;

public class BaseUnitTest
{
    public BaseUnitTest()
    {
        DateTimeProvider = new Mock<IDateTimeProvider>();
        DateTimeProvider.SetupGet(x => x.UtcNow)
            .Returns(() => DateTime.Now);
    }

    public Mock<IRedisCache> PassthroughCache { get; set; } = CacheMockHelper.CreatePassthroughCache();
    
    public Mock<IDateTimeProvider> DateTimeProvider { get; set; }
}