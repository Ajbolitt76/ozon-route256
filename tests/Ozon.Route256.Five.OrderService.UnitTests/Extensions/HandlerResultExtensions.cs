using FluentAssertions;
using FluentAssertions.Primitives;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;

namespace Ozon.Route256.Five.OrderService.UnitTests.Extensions;

public static class HandlerResultExtensions
{
    public static HandlerResultAssertions Should(this HandlerResult? result)
    {
        new ObjectAssertions(result).NotBeNull();
        return new(result!);
    }

    public static HandlerResultAssertions<T> Should<T>(this HandlerResult<T>? result)
    {
        new ObjectAssertions(result).NotBeNull();
        return new(result!);
    }
}