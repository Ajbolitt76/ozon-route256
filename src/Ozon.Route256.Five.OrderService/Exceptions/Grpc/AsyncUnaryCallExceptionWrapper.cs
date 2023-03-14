using Grpc.Core;

namespace Ozon.Route256.Five.OrderService.Exceptions.Grpc;

public class AsyncUnaryCallExceptionWrapper<TValue> : GenericTaskExceptionWrapper<AsyncUnaryCall<TValue>, TValue>
{
    public AsyncUnaryCallExceptionWrapper(AsyncUnaryCall<TValue> call) : base(call)
    {
    }
    
    public override TaskErrorWrapperAwaiter<TValue> GetAwaiter()
        => new(InnerAwaitable.ResponseAsync);
}