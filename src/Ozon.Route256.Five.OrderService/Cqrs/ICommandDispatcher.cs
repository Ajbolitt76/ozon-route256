using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;

namespace Ozon.Route256.Five.OrderService.Cqrs;

public interface ICommandDispatcher
{
    Task<HandlerResult<TCommandResult>> Dispatch<TCommand, TCommandResult>(TCommand command, CancellationToken cancellation)
        where TCommand : ICommand<TCommandResult>;
    
    Task<HandlerResult> Dispatch<TCommand>(TCommand command, CancellationToken cancellation) 
        where TCommand : ICommand;
}