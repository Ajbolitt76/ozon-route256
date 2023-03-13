using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Grpc.Core;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;
using Ozon.Route256.Five.OrderService.Exceptions.Grpc;

namespace Ozon.Route256.Five.OrderService.Exceptions;

public record struct TaskErrorWrapperAwaiter<TResult> : ICriticalNotifyCompletion, INotifyCompletion
{
    private readonly Task<TResult> _task;
    private readonly TaskAwaiter<TResult> _taskAwaiter;

    public TaskErrorWrapperAwaiter(Task<TResult> task)
    {
        _task = task;
        _taskAwaiter = task.GetAwaiter();
    }

    public bool IsCompleted => _task.IsCompleted;
    
    public HandlerResult<TResult> GetResult()
    {
        if (!_task.IsCompleted)
            _task.Wait(Timeout.Infinite, default);

        if (!_task.IsCompletedSuccessfully)
        {
            if (_task.IsFaulted)
                return HandlerResult<TResult>.FromError(ConvertToDomainOrThrow(_task.Exception!.InnerExceptions[0]));
            if (_task.IsCanceled)
                throw new TaskCanceledException(_task);
        }

        return HandlerResult<TResult>.FromValue(_task.Result);
    }

    public void OnCompleted(Action continuation) => _taskAwaiter.OnCompleted(continuation);

    public void UnsafeOnCompleted(Action continuation) => _taskAwaiter.UnsafeOnCompleted(continuation);

    // Если ошибка может быть отображено в домен, то отображаем, иначе кидаем
    private static DomainException ConvertToDomainOrThrow(Exception? exception)
    {
        if (exception is DomainException dex)
            return dex;
        if (exception is RpcException rpcException)
            return rpcException.ToDomain() ?? throw rpcException;

        throw exception;
    }
}

public record struct TaskErrorWrapperAwaiter : ICriticalNotifyCompletion, INotifyCompletion
{
    private readonly Task _task;
    private readonly TaskAwaiter _taskAwaiter;

    public TaskErrorWrapperAwaiter(Task task)
    {
        _task = task;
        _taskAwaiter = task.GetAwaiter();
    }

    public bool IsCompleted => _task.IsCompleted;
    
    public HandlerResult GetResult()
    {
        if (!_task.IsCompleted)
            _task.Wait(Timeout.Infinite, default);

        if (!_task.IsCompletedSuccessfully)
        {
            if (_task.IsFaulted)
                return HandlerResult.FromError(ConvertToDomainOrThrow(_task.Exception!.InnerExceptions[0]));
            if (_task.IsCanceled)
                throw new TaskCanceledException(_task);
        }

        return HandlerResult.Ok;
    }

    public void OnCompleted(Action continuation) => _taskAwaiter.OnCompleted(continuation);

    public void UnsafeOnCompleted(Action continuation) => _taskAwaiter.UnsafeOnCompleted(continuation);

    // Если ошибка может быть отображено в домен, то отображаем, иначе кидаем
    private static DomainException ConvertToDomainOrThrow(Exception? exception)
    {
        if (exception is DomainException dex)
            return dex;
        if (exception is RpcException rpcException)
        {
            var domain = rpcException.ToDomain();
            if(domain is null)
                ExceptionDispatchInfo.Throw(rpcException);
            return domain;
        }

        throw exception;
    }
}