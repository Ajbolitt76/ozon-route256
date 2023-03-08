namespace Ozon.Route256.Five.OrderService.Cqrs;

public interface IQuery<TResult> : IRequest<TResult>
{
}

public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    
}