using FluentAssertions;
using Ozon.Route256.Five.OrderService.Mappings;
using Ozon.Route256.Five.OrderService.Services.DbClientBalancer;
using Ozon.Route256.Five.Sd.Grpc;

namespace Ozon.Route256.Five.OrderService.UnitTests.Mappings;

public class ReplicaMappingsTest
{
    /// <summary>
    /// Маппинг должен обрабатывать все варианты grpc контракта
    /// </summary>
    [Fact]
    public void Map_ReplicaType_ShouldCoverAllGrpcVariants()
    {
        var variants = Enum.GetValues<Replica.Types.ReplicaType>();
        var knownMaps = new Dictionary<Replica.Types.ReplicaType, DbReplicaType>()
        {
            [Replica.Types.ReplicaType.Async] = DbReplicaType.Async,
            [Replica.Types.ReplicaType.Master] = DbReplicaType.Master,
            [Replica.Types.ReplicaType.Sync] = DbReplicaType.Sync
        };
        
        var mapped = variants.ToDictionary(x => x, x => x.ToModel());

        mapped.Should()
            .HaveSameCount(variants)
            .And
            .Equal(knownMaps);
    }    
}