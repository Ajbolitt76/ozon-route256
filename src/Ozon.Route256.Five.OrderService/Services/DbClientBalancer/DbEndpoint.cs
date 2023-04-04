namespace Ozon.Route256.Five.OrderService.Services.DbClientBalancer;

public record DbEndpoint(string HostPort, DbReplicaType ReplicaType, int[] Buckets)
{
    public virtual bool Equals(DbEndpoint? other)
    {
        return other != null
               && EqualityComparer<string>.Default.Equals(HostPort, other.HostPort)
               && EqualityComparer<DbReplicaType>.Default.Equals(ReplicaType, other.ReplicaType)
               && Buckets.SequenceEqual(other.Buckets);
    }
}