namespace Ozon.Route256.Five.OrderService.Contracts.GetForRegions;

/// <summary>
/// Статистика для конкретного региона
/// </summary>
/// <param name="Region">Регион</param>
/// <param name="OrdersCount">Количество заказов</param>
/// <param name="CustomersCount">Количество уникальный клиентов</param>
/// <param name="TotalPrice">Общая сумма заказов</param>
/// <param name="TotalWeight">Суммарный вес</param>
public record GetForRegionsResponseItem(
    string Region, 
    long OrdersCount,
    long CustomersCount,
    decimal TotalPrice, 
    double TotalWeight);