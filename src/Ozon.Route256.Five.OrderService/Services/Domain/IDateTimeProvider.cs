namespace Ozon.Route256.Five.OrderService.Services.Domain;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
}