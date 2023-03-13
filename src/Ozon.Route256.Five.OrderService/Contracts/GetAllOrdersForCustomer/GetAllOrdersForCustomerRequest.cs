namespace Ozon.Route256.Five.OrderService.Contracts.GetAllOrdersForCustomer;

/// <summary>
/// Запрос на получение всех заказов клиента
/// </summary>
/// <param name="ClientId">Id клиента</param>
/// <param name="StartFrom">Дата начала аггрегации</param>
/// <param name="PageNumber">Номер страницы</param>
/// <param name="PageSize">Размер страницы</param>
public record GetAllOrdersForCustomerRequest(
    int ClientId,
    DateTime? StartFrom,
    int PageNumber,
    int PageSize)
{
    public int PageNumber { get; } = PaginationDefaults.NormalizePageNumber(PageNumber);

    public int PageSize { get; } = PaginationDefaults.NormalizePageSize(PageSize);
};