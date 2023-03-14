using System.Runtime.ExceptionServices;
using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Five.OrderService.Exceptions;

namespace Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;

public static class ResultExtensions
{
    /// <summary>
    /// Returns different values, depend on <see cref="HandlerResult"/> result
    /// </summary>
    /// <param name="handlerResult"></param>
    /// <param name="successDelegate">Return if success</param>
    /// <param name="failedDelegate">Return if failed</param>
    /// <returns></returns>
    public static TResult Handle<TResult>(this HandlerResult handlerResult, Func<TResult> successDelegate, Func<DomainException, TResult> failedDelegate) => 
        handlerResult.Success ? successDelegate() : failedDelegate(handlerResult.Error);

    /// <summary>
    /// Returns different values, depend on <see cref="HandlerResult"/> result
    /// </summary>
    /// <param name="handlerResult"></param>
    /// <param name="successDelegate">Return if success</param>
    /// <param name="failedDelegate">Return if failed</param>
    /// <returns></returns>
    public static TResult Handle<TResult, T>(this HandlerResult<T> handlerResult, Func<T, TResult> successDelegate, Func<DomainException, TResult> failedDelegate) where T : class => 
        handlerResult.Success ? successDelegate(handlerResult.Value) : failedDelegate(handlerResult.Error);

    /// <summary>
    /// Монадический bind для HandlerResult
    /// </summary>
    /// <param name="handlerResult">Результат хэндлера</param>
    /// <param name="map">Стрелка клейсли</param>
    public static HandlerResult<TResult> Bind<TResult, T>(this HandlerResult<T> handlerResult, Func<T, HandlerResult<TResult>> map)
        => handlerResult switch
        {
            { Success: true, Value: var val } => map(val),
            { Success: false, Error: var err } => HandlerResult<TResult>.FromError(err)
        };
    
    public static IActionResult ToActionResult<T>(this HandlerResult<T> handlerResult)
        => handlerResult switch
        {
            { Success: true, Value: var val } => new JsonResult(val),
            { Success: false, Error: var err } => ExceptionHelper.ToActionResult(err)
        };
    
    public static IActionResult ToActionResult(this HandlerResult handlerResult)
        => handlerResult switch
        {
            { Success: true } => new OkResult(),
            { Success: false, Error: var err } => ExceptionHelper.ToActionResult(err)
        };

    /// <summary>
    /// Checks if the <see cref="HandlerResult"/> was not successful and throws the underlying exception, if available.
    /// While re-throwing, it preserves the StackTrace. Returns the Result if successful, so that it can be quickly checked.
    /// </summary>
    /// <param name="handlerResult"></param>
    public static HandlerResult Check(this HandlerResult handlerResult)
    {
        if (handlerResult.Success) 
            return handlerResult;
        var error = handlerResult.Error;
        PreserveStackTrace(error);
        throw error;
    }

    /// <summary>
    /// Checks if the <see cref="HandlerResult"/> was not successful and throws the underlying exception, if available.
    /// While re-throwing, it preserves the StackTrace. Returns the Result if successful, so that it can be quickly checked.
    /// </summary>
    /// <param name="handlerResult"></param>
    public static HandlerResult<T> Check<T>(this HandlerResult<T> handlerResult) where T : class
    {
        if (handlerResult.Success) 
            return handlerResult;
        
        var error = handlerResult.Error;
        PreserveStackTrace(error);
        throw error;
    }
    
    private static void PreserveStackTrace(Exception exception)
    {
        ExceptionDispatchInfo.Capture(exception).Throw();
    }
}
