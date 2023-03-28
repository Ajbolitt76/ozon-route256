using System.Data.Common;
using Npgsql;

namespace Ozon.Route256.Five.OrderService.Services.Database;

public class ConnectionFactory : IConnectionFactory
{
    private readonly string _connectionString;

    public ConnectionFactory(
        DatabaseConnectionString dbConnections)
    {
        // После 6 ДЗ тут будет serviceDiscovery
        _connectionString = dbConnections.ConnectionString;
    }
    
    public Task<DbConnection> GetConnectionAsync()
        => Task.FromResult<DbConnection>(new NpgsqlConnection(_connectionString));
}