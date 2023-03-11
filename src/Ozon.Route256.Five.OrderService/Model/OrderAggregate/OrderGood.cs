namespace Ozon.Route256.Five.OrderService.Model;

public record OrderGood(
    long Id,
    string Name,
    int Quantity,
    decimal Price,
    double Weight);