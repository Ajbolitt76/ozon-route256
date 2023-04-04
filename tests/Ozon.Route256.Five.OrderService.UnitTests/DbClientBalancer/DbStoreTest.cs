using FluentAssertions;
using Ozon.Route256.Five.OrderService.Services.DbClientBalancer;

namespace Ozon.Route256.Five.OrderService.UnitTests.DbClientBalancer;

public class DbStoreTest
{
    [Fact]
    public async Task Call_SetEndpointList_ShouldUpdateList()
    {
        // Arrange
        var dbStore = new DbStore();
        var endpoints = new List<DbEndpoint>()
        {
            new("test1", DbReplicaType.Async, new[] { 0, 1, 2 }),
            new("test2", DbReplicaType.Master, new[] { 3, 4, 5 }),
        };

        // Act
        await dbStore.SetEndpointList(endpoints);

        // Assert
        dbStore.Endpoints.Should().Equal(endpoints);
    }

    [Fact]
    public async Task Call_GetForBucketAsync_ShouldReturnConnection()
    {
        // Arrange

        var dbStore = new DbStore();
        var endpoints = new List<DbEndpoint>()
        {
            new("test1", DbReplicaType.Async, new[] { 0, 1, 2 }),
            new("test2", DbReplicaType.Master, new[] { 3, 4, 5 }),
            new("test3", DbReplicaType.Master, new[] { 6, 7, 8 }),
        };
        await dbStore.SetEndpointList(endpoints);

        // Act
        var results = await Enumerable.Range(0, 9)
            .ToAsyncEnumerable()
            .SelectAwait(
                async x => (
                    BucketId: x,
                    ResultConnection: await dbStore.GetForBucketAsync(x, default)))
            .ToListAsync();

        // Assert 
        results.Should()
            .AllSatisfy(
                x =>
                    x.ResultConnection?.Buckets
                        .Should()
                        .NotBeNull()
                        .And
                        .Contain(x.BucketId));
    }
}