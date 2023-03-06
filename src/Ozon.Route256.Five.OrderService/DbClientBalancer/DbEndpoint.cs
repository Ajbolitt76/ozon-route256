namespace Ozon.Route256.Five.OrderService.DbClientBalancer;

public record DbEndpoint(string ConnectionString, DbReplicaType ReplicaType);
