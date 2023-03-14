using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Five.OrderService.Contracts;

namespace Ozon.Route256.Five.OrderService.Exceptions;

public static class ExceptionHelper
{
    public static ErrorDescription ToDescription(DomainException exception)
        => new(exception.Code, exception.Message);

    public static RpcException ToRpcException(this DomainException exception)
        => exception switch
        {
            NotFoundException ex => new RpcException(new Status(StatusCode.NotFound, ex.Message, ex)),
            _ => new RpcException(new Status(StatusCode.FailedPrecondition, exception.Message, exception))
        };
    
    public static IActionResult ToActionResult(DomainException exception)
        => exception switch
        {
            NotFoundException => new NotFoundObjectResult(ToDescription(exception)),
            _ => new BadRequestObjectResult(ToDescription(exception))
        };
}