namespace Ozon.Route256.Five.OrderService.Cqrs;

public interface ICommand<TResult> : IRequest<TResult>
{
}

public interface ICommand : IRequest
{
}
