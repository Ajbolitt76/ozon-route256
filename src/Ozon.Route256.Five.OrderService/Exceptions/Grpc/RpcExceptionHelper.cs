using Grpc.Core;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ozon.Route256.Five.OrderService.Exceptions.Grpc;

public static class RpcExceptionHelper
{
    /// <summary>
    /// Обернуть RpcException, которые мы знаем как обернуть, в доменные исключения
    /// </summary>
    /// <param name="exception"></param>
    /// <returns>Доменное исключение или null</returns>
    public static DomainException? ToDomain(this RpcException exception)
        => exception switch
        {
            { StatusCode: StatusCode.NotFound } => new NotFoundException(exception.Message),
            _ => null
        };

    public static AsyncUnaryCallExceptionWrapper<TValue> ToHandlerResult<TValue>(this AsyncUnaryCall<TValue> call)
        => new(call);
}