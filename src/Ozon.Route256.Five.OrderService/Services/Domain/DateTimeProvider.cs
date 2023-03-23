namespace Ozon.Route256.Five.OrderService.Services.Domain;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}