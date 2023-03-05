namespace Ozon.Route256.Five.OrderService.Contracts.GetAllOrdersForClient;

/// <summary>
/// Запрос на получение всех заказов клиента
/// </summary>
/// <param name="ClientId">Id клиента</param>
/// <param name="StartFrom">Дата начала аггрегации</param>
/// <param name="PageNumber">Номер страницы</param>
/// <param name="PageSize">Размер страницы</param>
public record GetAllOrdersForClientRequest(
    long ClientId,
    DateTime? StartFrom,
    int PageNumber,
    int PageSize)
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
};