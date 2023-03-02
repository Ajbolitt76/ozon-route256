namespace Ozon.Route256.Five.OrderService.Contracts.GetOrders;

public record GetOrdersRequest(List<string>? Regions)
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

    public bool IsAscending { get; set; }
}