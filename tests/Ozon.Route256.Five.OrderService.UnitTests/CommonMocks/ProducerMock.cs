using Moq;
using Ozon.Route256.Five.OrderService.Services.Kafka.Producers;

namespace Ozon.Route256.Five.OrderService.UnitTests.CommonMocks;

public class ProducerMock<TKey, TMessage>
{
    private readonly Mock<IMessageProducer<TKey, TMessage>> _mock;

    private readonly List<(TKey Key, TMessage Message)> _sentMessages = new();

    public IReadOnlyList<(TKey Key, TMessage Message)> SentMessages => _sentMessages;

    public IMessageProducer<TKey, TMessage> Object => _mock.Object;

    public ProducerMock()
    {
        _mock = new Mock<IMessageProducer<TKey, TMessage>>();

        _mock.Setup(x => x.Send(
                It.IsAny<TKey>(), 
                It.IsAny<TMessage>(), 
                It.IsAny<CancellationToken>()))
            .Callback((TKey key, TMessage message, CancellationToken _) =>
            {
                _sentMessages.Add((key, message));
            })
            .Returns(Task.CompletedTask);
    }
}