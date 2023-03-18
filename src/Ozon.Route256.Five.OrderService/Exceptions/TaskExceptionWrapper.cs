using Ozon.Route256.Five.OrderService.Exceptions.Grpc;

namespace Ozon.Route256.Five.OrderService.Exceptions;

public class TaskExceptionWrapper<TValue> : GenericTaskExceptionWrapper<Task<TValue>, TValue>
{
    public TaskExceptionWrapper(Task<TValue> call) : base(call)
    {
    }

    public override TaskErrorWrapperAwaiter<TValue> GetAwaiter()
        => new(InnerAwaitable);
}

public class TaskExceptionWrapper : GenericTaskExceptionWrapper<Task>
{
    public TaskExceptionWrapper(Task call) : base(call)
    {
    }

    public override TaskErrorWrapperAwaiter GetAwaiter()
        => new(InnerAwaitable);
}