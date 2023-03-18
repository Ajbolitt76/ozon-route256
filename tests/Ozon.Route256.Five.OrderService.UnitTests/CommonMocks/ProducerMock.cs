using Moq;
using Ozon.Route256.Five.OrderService.Services.Kafka.Producers;

namespace Ozon.Route256.Five.OrderService.UnitTests.CommonMocks;

public class ProducerMock<T>
{
    private readonly Mock<IProducer<T>> _mock;

    private readonly List<T> _sentMessages = new();

    public IReadOnlyList<T> SentMessages => _sentMessages;
    
    public IProducer<T> Object => _mock.Object;
    
    public ProducerMock()
    {
        _mock = new Mock<IProducer<T>>();
        
        _mock.Setup(x => x.Send(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .Callback((T x, CancellationToken _) => _sentMessages.Add(x))
            .Returns(Task.CompletedTask);
    }
}