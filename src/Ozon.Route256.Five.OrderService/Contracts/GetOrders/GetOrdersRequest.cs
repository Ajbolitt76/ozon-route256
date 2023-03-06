namespace Ozon.Route256.Five.OrderService.Contracts.GetOrders;

/// <summary>
/// Запрос на получение заказов
/// </summary>
/// <param name="Regions">Регионы для которых идет агрегация</param>
/// <param name="IsAscending">По возрастанию</param>
/// <param name="PageNumber">Номер страницы</param>
/// <param name="PageSize">Размер страницы</param>
public record GetOrdersRequest(List<string>? Regions, bool? IsAscending, int PageNumber, int PageSize)
{
    private int _pageNumber;
    private int _pageSize;
    
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value > 0 ? value : PaginationDefaults.PageNumber;
    }
    
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > 0 ? value : PaginationDefaults.PageSize;
    }
}