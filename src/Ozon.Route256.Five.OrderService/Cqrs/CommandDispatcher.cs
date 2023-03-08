using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;

namespace Ozon.Route256.Five.OrderService.Cqrs;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<HandlerResult<TCommandResult>> Dispatch<TCommand, TCommandResult>(TCommand command,
        CancellationToken cancellation) where TCommand : ICommand<TCommandResult>
    {
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TCommandResult>>();
        return handler.Handle(command, cancellation);
    }

    public Task<HandlerResult> Dispatch<TCommand>(TCommand command, CancellationToken cancellation)
        where TCommand : ICommand
    {
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        return handler.Handle(command, cancellation);
    }
}