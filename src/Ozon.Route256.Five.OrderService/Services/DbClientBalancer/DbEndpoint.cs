namespace Ozon.Route256.Five.OrderService.Services.DbClientBalancer;

public record DbEndpoint(string ConnectionString, DbReplicaType ReplicaType);
