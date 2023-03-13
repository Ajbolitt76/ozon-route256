namespace Ozon.Route256.Five.OrderService.Contracts.GetOrders;

/// <summary>
/// Запрос на получение заказов
/// </summary>
/// <param name="Regions">Регионы для которых идет агрегация</param>
/// <param name="IsAscending">По возрастанию</param>
/// <param name="PageNumber">Номер страницы</param>
/// <param name="PageSize">Размер страницы</param>
public record GetOrdersRequest(IReadOnlyList<string>? Regions, bool? IsAscending, int PageNumber, int PageSize)
{
    public int PageNumber { get; } = PaginationDefaults.NormalizePageNumber(PageNumber);

    public int PageSize { get; } = PaginationDefaults.NormalizePageSize(PageSize);
}