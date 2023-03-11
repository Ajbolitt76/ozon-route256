using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;
using Ozon.Route256.Five.OrderService.Exceptions;

namespace Ozon.Route256.Five.OrderService.UnitTests.Extensions;

public class HandlerResultAssertions
{
    private readonly HandlerResult _result;

    public HandlerResultAssertions(HandlerResult result)
    {
        _result = result;
    }

    public AndConstraint<HandlerResultAssertions> BeSuccessful(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(_result.Success)
            .FailWith("Expected success result, but got failure");

        return new AndConstraint<HandlerResultAssertions>(this);
    }

    public AndWhichConstraint<ObjectAssertions, T> FailWithStrict<T>(string because = "", params object[] becauseArgs) where T : DomainException
    {
        Fail(because, becauseArgs);
        return new ObjectAssertions(_result.Error).BeOfType<T>();
    }

    public AndWhichConstraint<ObjectAssertions, T> FailWith<T>(string because = "", params object[] becauseArgs) where T : DomainException
    {
        Fail(because, becauseArgs);
        return new ObjectAssertions(_result.Error).BeAssignableTo<T>();
    }
    
    public void Fail(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(!_result.Success)
            .FailWith("Expected failure result, but got success");
    }
}


public class HandlerResultAssertions<T>
{
    private readonly HandlerResult<T?> _result;

    public HandlerResultAssertions(HandlerResult<T?> result)
    {
        _result = result;
    }

    public HandlerResultAssertions<T> BeSuccessful(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(_result.Success)
            .FailWith("Expected success result, but got failure. {0}", _result);

        return this;
    }

    public T? WhichResult()
        => _result.Value;
    
    public T WhichRequiredResult()
    {
        _result.Value.Should().NotBeNull();
        return _result.Value!;
    }


    public AndWhichConstraint<ObjectAssertions, TError> FailWith<TError>() where TError : DomainException
    {
        Fail();
        return new ObjectAssertions(_result.Error).BeAssignableTo<TError>();
    }
    
    public AndWhichConstraint<ObjectAssertions, TError> FailWithStrict<TError>() where TError : DomainException
    {
        Fail();
        return new ObjectAssertions(_result.Error).BeOfType<TError>();
    }
    
    public void Fail(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(!_result.Success)
            .FailWith("Expected failure result, but got success {0}", _result );
    }
}