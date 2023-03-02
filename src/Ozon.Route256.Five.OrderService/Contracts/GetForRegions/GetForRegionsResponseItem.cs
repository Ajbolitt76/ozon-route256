namespace Ozon.Route256.Five.OrderService.Contracts.GetForRegions;

public record GetForRegionsResponseItem(
    string Region, 
    int OrdersCount,
    int CustomersCount,
    Decimal TotalPrice, 
    double TotalWeight);