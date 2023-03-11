using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;
using Ozon.Route256.Five.OrderService.Exceptions;

namespace Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;

public record HandlerResult
{
    public DomainException? Error { get; }

    [MemberNotNullWhen(false, nameof(Error))]
    public bool Success { get; }

    private HandlerResult(bool success, DomainException? error)
    {
        Success = success;
        Error = error;
    }

    public static HandlerResult Ok => new(true, null);

    public static HandlerResult FromError(DomainException error) => new(false, error);

    public static implicit operator HandlerResult(DomainException exception) => new(false, exception);
}

public record HandlerResult<TValue>
{
    public TValue? Value { get; }
    public DomainException? Error { get; }

    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool Success { get; }

    private HandlerResult(TValue? value, bool success, DomainException? error)
    {
        Value = value;
        Success = success;
        Error = error;
    }

    private HandlerResult(TValue value) : this(value, true, null)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    private HandlerResult(DomainException innerError) : this(default, false, innerError)
    {
    }

    public static HandlerResult<TValue> FromValue(TValue payload) => new(payload);

    public static HandlerResult<TValue> FromError(DomainException error) => new(error);

    public static implicit operator HandlerResult<TValue>(TValue payload) => new(payload);

    public static implicit operator HandlerResult<TValue>(DomainException exception) =>
        new(exception);
}