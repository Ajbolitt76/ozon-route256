using System.Runtime.CompilerServices;
using Grpc.Core;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;

namespace Ozon.Route256.Five.OrderService.Exceptions.Grpc;

public abstract class GenericTaskExceptionWrapper<TAwaitable, TValue>
{
    public GenericTaskExceptionWrapper(TAwaitable call)
    {
        InnerAwaitable = call;
    }

    public TAwaitable InnerAwaitable { get; }

    public abstract TaskErrorWrapperAwaiter<TValue> GetAwaiter();
}

public abstract class GenericTaskExceptionWrapper<TAwaitable>
{
    public GenericTaskExceptionWrapper(TAwaitable call)
    {
        InnerAwaitable = call;
    }

    public TAwaitable InnerAwaitable { get; }

    public abstract TaskErrorWrapperAwaiter GetAwaiter();
}

public class AsyncUnaryCallExceptionWrapper<TValue> : GenericTaskExceptionWrapper<AsyncUnaryCall<TValue>, TValue>
{
    public AsyncUnaryCallExceptionWrapper(AsyncUnaryCall<TValue> call) : base(call)
    {
    }
    
    public override TaskErrorWrapperAwaiter<TValue> GetAwaiter()
        => new(InnerAwaitable.ResponseAsync);
}