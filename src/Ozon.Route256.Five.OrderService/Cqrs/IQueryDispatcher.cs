using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;

namespace Ozon.Route256.Five.OrderService.Cqrs;

public interface IQueryDispatcher
{
    Task<HandlerResult<TQueryResult>> Dispatch<TQuery, TQueryResult>(TQuery query, CancellationToken cancellation)
        where TQuery : IQuery<TQueryResult>;
    
}