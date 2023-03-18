using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Ozon.Route256.Five.OrderService;

public partial class LoggerInterceptor: Interceptor
{
    private readonly ILogger<LoggerInterceptor> _logger;

    public LoggerInterceptor(ILogger<LoggerInterceptor> logger)
    {
        _logger = logger;
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        LogOutcoming(context.Method.Name, request);
        return base.AsyncUnaryCall(request, context, continuation);
    }

    public override TResponse BlockingUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        LogOutcoming(context.Method.Name, request);
        return base.BlockingUnaryCall(request, context, continuation);
    }

    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        LogIncoming(context.Method, request);
        return base.UnaryServerHandler(request, context, continuation);
    }

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Входящий вызов {handlerName}\n {request}")]
    private partial void LogIncoming(string handlerName, object request);
    
    
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Исходящий вызов {handlerName}.\n {request}")]
    private partial void LogOutcoming(string handlerName, object request);
}