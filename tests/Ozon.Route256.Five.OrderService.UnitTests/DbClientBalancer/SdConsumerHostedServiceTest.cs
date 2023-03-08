using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Moq;
using Ozon.Route256.Five.OrderService.DbClientBalancer;
using Ozon.Route256.Five.OrderService.Mappings;
using Ozon.Route256.Five.OrderService.UnitTests.CommonMocks;
using Ozon.Route256.Five.OrderService.UnitTests.Grpc;
using Ozon.Route256.Five.Sd.Grpc;
using Xunit.Abstractions;

namespace Ozon.Route256.Five.OrderService.UnitTests.DbClientBalancer;

public class SdConsumerHostedServiceTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const int DbStoreMaxDelay = 100;

    public SdConsumerHostedServiceTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// Сообщения из открытого потока, должны обрабатывться и сохранятся в DbStore
    /// </summary>
    [Fact]
    public async Task Handle_IncomingStream_ShouldModifyDbStore()
    {
        var serverStreamMock = new GrpcServerStreamPublisher<DbResourcesResponse>();

        var clientMock = new Mock<SdService.SdServiceClient>(MockBehavior.Strict);
        clientMock.Setup(
                x => x.DbResources(
                    It.IsAny<DbResourcesRequest>(),
                    It.IsAny<Metadata?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()))
            .Returns(() => serverStreamMock.Call);

        var savedCallsHistory = new List<IReadOnlyCollection<DbEndpoint>>();
        var dbStoreMock = new Mock<IDbStore>();
        dbStoreMock
            .Setup(x => x.SetEndpointList(It.IsAny<IReadOnlyCollection<DbEndpoint>>()))
            .Returns(() => Task.Delay(Random.Shared.Next(0, DbStoreMaxDelay)))
            .Callback((IReadOnlyCollection<DbEndpoint> x) => savedCallsHistory.Add(x));

        var service = new SdConsumerHostedService(
            dbStoreMock.Object,
            clientMock.Object,
            LoggerMock.GetILogger<SdConsumerHostedService>().Object);

        var batches = new DbResourcesResponse[]
        {
            new()
            {
                ClusterName = "clusterName",
                LastUpdated = Timestamp.FromDateTime(new DateTime(2022, 12, 2, 10, 23, 00, DateTimeKind.Utc)),
                Replicas =
                {
                    new Replica()
                    {
                        Host = "testHost",
                        Port = 6000,
                        Type = Replica.Types.ReplicaType.Async
                    },
                    new Replica()
                    {
                        Host = "localMost",
                        Port = 6100,
                        Type = Replica.Types.ReplicaType.Sync
                    }
                }
            },
            new()
            {
                ClusterName = "clusterName",
                LastUpdated = Timestamp.FromDateTime(new DateTime(2022, 12, 2, 12, 23, 00, DateTimeKind.Utc)),
                Replicas =
                {
                    new Replica()
                    {
                        Host = "kis",
                        Port = 6230,
                        Type = Replica.Types.ReplicaType.Master
                    },
                }
            }
        };

        await service.StartAsync(default);

        foreach (var batch in batches)
            await serverStreamMock.PacketWriter.WriteAsync(batch);

        await Task.Delay(DbStoreMaxDelay * batches.Length + 10);

        await service.StopAsync(default);

        // Проверяем, что DbSrtore.SetEndpointList вызван для каждого сообщения с корректными данными,
        savedCallsHistory
            .Should()
            .SatisfyRespectively(batches.Select(CreateDbCheckerForDbResourcesResponse));
    }

    /// <summary>
    /// При ошибке во время первого запроса или ошибке при стриме должен переподключиться
    /// </summary>
    [Fact]
    public async Task Handle_RpcErrorWhenConnecting_ShouldRetry()
    {
        var badStreamMock = new GrpcServerStreamPublisher<DbResourcesResponse>();
        var goodStreamMock = new GrpcServerStreamPublisher<DbResourcesResponse>();

        var clientMock = new Mock<SdService.SdServiceClient>(MockBehavior.Strict);
        clientMock.SetupSequence(
                x => x.DbResources(
                    It.IsAny<DbResourcesRequest>(),
                    It.IsAny<Metadata?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()))
            .Throws(new RpcException(new Status(StatusCode.DataLoss, "Error")))
            .Returns(badStreamMock.Call)
            .Returns(goodStreamMock.Call);

        var dbStoreMock = new Mock<IDbStore>();
        dbStoreMock
            .Setup(x => x.SetEndpointList(It.IsAny<IReadOnlyCollection<DbEndpoint>>()))
            .Returns(() => Task.CompletedTask);

        var service = new SdConsumerHostedService(
            dbStoreMock.Object,
            clientMock.Object,
            LoggerMock.GetILogger<SdConsumerHostedService>().Object);

        await service.StartAsync(default);

        //Первый ретрай
        await Task.Delay(1300);

        //Второй ретрай
        badStreamMock.PacketWriter.Complete(new RpcException(new Status(StatusCode.ResourceExhausted, "Test")));
        await Task.Delay(1300);

        await service.StopAsync(default);

        clientMock.Verify(
            x => x.DbResources(
                It.IsAny<DbResourcesRequest>(),
                It.IsAny<Metadata?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(3));
    }

    private Action<IEnumerable<DbEndpoint>> CreateDbCheckerForDbResourcesResponse(DbResourcesResponse response)
    {
        var dbEndpoints = response
            .Replicas
            .Select(x => new DbEndpoint($"{x.Host}:{x.Port}", x.Type.ToModel()))
            .ToArray();

        return (entry) => entry.Should().Equal(dbEndpoints);
    }
}