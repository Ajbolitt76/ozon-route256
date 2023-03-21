using Moq;
using Ozon.Route256.Five.OrderService.Services.Domain;
using Ozon.Route256.Five.OrderService.Services.Redis;
using Ozon.Route256.Five.OrderService.UnitTests.CommonMocks;

namespace Ozon.Route256.Five.OrderService.UnitTests;

public abstract class BaseUnitTest
{
    protected BaseUnitTest()
    {
        
    }
    
    public Mock<IRedisCache> PassthroughCache { get; set; } = CacheMockHelper.CreatePassthroughCache();
}