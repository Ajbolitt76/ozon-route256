namespace Ozon.Route256.Five.OrderService.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base("NOT_FOUND", message)
    {
    }

    public static NotFoundException WithStandardMessage<TEntity>(object id)
        => new($"Entity {typeof(TEntity).Name} with {id} not found");
}