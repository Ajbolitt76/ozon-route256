namespace Ozon.Route256.Five.OrderService.Cqrs;

public interface IRequest
{
}

public interface IRequest<TResponse> : IRequest
{
}