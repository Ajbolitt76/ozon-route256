namespace Ozon.Route256.Five.OrderService.Contracts.GetForRegions;

/// <summary>
/// Статистика для конкретного региона
/// </summary>
/// <param name="Region">Регион</param>
/// <param name="OrdersCount">Количество заказов</param>
/// <param name="CustomersCount">Количество уникальный клиентов</param>
/// <param name="total_price">Общая сумма заказов</param>
/// <param name="TotalWeight">Суммарный вес</param>
public record GetForRegionsResponseItem(
    string Region, 
    int OrdersCount,
    int CustomersCount,
    Decimal total_price, 
    double TotalWeight);