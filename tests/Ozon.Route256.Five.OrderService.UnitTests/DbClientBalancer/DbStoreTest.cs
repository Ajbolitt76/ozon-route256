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
            new("test1", DbReplicaType.Async),
            new("test2", DbReplicaType.Master),
        };

        // Act
        await dbStore.SetEndpointList(endpoints);

        // Assert
        dbStore.Endpoints.Should().Equal(endpoints);
    }

    [Fact]
    public async Task Call_GetNextDbEndpointAsync_ShouldRoundRobin()
    {
        // Arrange
        
        var dbStore = new DbStore();
        var endpoints = new List<DbEndpoint>()
        {
            new("test1", DbReplicaType.Async),
            new("test2", DbReplicaType.Master),
            new("test3", DbReplicaType.Master),
        };
        await dbStore.SetEndpointList(endpoints);

        // Act
        var results = await Enumerable.Range(0, endpoints.Count * 2)
            .ToAsyncEnumerable()
            .SelectAwait(async _ => await dbStore.GetNextDbEndpointAsync())
            .ToListAsync();

        // Assert 
        results.Should().Equal(Enumerable.Range(0, 2).SelectMany(_ => endpoints));
    }
}