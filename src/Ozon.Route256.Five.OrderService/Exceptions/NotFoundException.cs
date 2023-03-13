namespace Ozon.Route256.Five.OrderService.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base("NOT_FOUND", message)
    {
    }

    public static NotFoundException WithStandardMessage<TEntity>()
        => new($"Entity {typeof(TEntity).Name} not found");
}