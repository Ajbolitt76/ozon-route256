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