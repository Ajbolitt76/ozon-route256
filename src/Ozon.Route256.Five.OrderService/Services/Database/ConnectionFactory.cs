using System.Data.Common;
using Npgsql;

namespace Ozon.Route256.Five.OrderService.Services.Database;

public class ConnectionFactory : IConnectionFactory
{
    private readonly IHostEnvironment _environment;
    private readonly ILoggerFactory _loggerFactory;
    private readonly string _connectionString;

    public ConnectionFactory(
        IHostEnvironment environment,
        ILoggerFactory loggerFactory,
        IConfiguration configuration)
    {
        _environment = environment;
        _loggerFactory = loggerFactory;
        
        // После 6 ДЗ тут будет serviceProvider
        _connectionString = configuration.GetConnectionString("DbConnection")
            ?? throw new ApplicationException("Не указанна строка подключения к PG");
    }
    
    public Task<DbConnection> GetConnectionAsync()
        => Task.FromResult<DbConnection>(new NpgsqlConnection(_connectionString));
}