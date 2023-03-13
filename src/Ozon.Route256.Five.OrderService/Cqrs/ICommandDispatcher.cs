using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;

namespace Ozon.Route256.Five.OrderService.Cqrs;

public interface ICommandDispatcher
{
    Task<HandlerResult> Dispatch<TCommand>(TCommand command, CancellationToken cancellation) 
        where TCommand : ICommand;
}