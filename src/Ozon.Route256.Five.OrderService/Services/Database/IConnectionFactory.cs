using System.Data.Common;

namespace Ozon.Route256.Five.OrderService.Services.Database;

public interface IConnectionFactory
{
    public Task<DbConnection> GetConnectionAsync();
}