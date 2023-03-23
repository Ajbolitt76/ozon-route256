using Ozon.Route256.Five.OrderService.Services.DbClientBalancer;
using Ozon.Route256.Five.Sd.Grpc;

namespace Ozon.Route256.Five.OrderService.Mappings;

public static class ReplicaMappings
{
    public static DbReplicaType ToModel(this Replica.Types.ReplicaType grpcReplicaType)
        => grpcReplicaType switch
        {
            Replica.Types.ReplicaType.Master => DbReplicaType.Master,
            Replica.Types.ReplicaType.Sync => DbReplicaType.Sync,
            Replica.Types.ReplicaType.Async => DbReplicaType.Async,
            _ => throw new ArgumentOutOfRangeException(nameof(grpcReplicaType), grpcReplicaType, null)
        };
}