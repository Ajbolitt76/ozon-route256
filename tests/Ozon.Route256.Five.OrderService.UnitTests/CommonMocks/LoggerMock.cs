using Microsoft.Extensions.Logging;
using Moq;

namespace Ozon.Route256.Five.OrderService.UnitTests.CommonMocks;

public static class LoggerMock
{
    public static Mock<ILogger<T>> GetILogger<T>() => new();
}