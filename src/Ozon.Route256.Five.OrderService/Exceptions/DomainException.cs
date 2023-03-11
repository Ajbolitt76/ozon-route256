namespace Ozon.Route256.Five.OrderService.Exceptions;

public class DomainException : ApplicationException
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string code, string message) : this(message)
    {
        Code = code;
    }

    public string? Code { get; protected set; }
}