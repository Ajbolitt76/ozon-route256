namespace Ozon.Route256.Five.OrderService.Exceptions;

public static class TaskExtensions
{
    public static TaskExceptionWrapper<TValue> ToHandlerResult<TValue>(this Task<TValue> task)
        => new(task);
    
    public static TaskExceptionWrapper ToHandlerResult(this Task task)
        => new(task);
}