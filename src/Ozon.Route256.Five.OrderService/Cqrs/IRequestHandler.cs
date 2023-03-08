using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;

namespace Ozon.Route256.Five.OrderService.Cqrs;


public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<HandlerResult<TResponse>> Handle(TRequest request, CancellationToken token);
}

public interface IRequestHandler<in TRequest> where TRequest : IRequest
{
    Task<HandlerResult> Handle(TRequest request, CancellationToken token);
}

public interface IRequestHandler : IRequestHandler<Unit>
{
    Task<HandlerResult> IRequestHandler<Unit>.Handle(Unit request, CancellationToken token) => Handle(token);
    Task<HandlerResult> Handle(CancellationToken token);
}

public record struct Unit : IRequest;
