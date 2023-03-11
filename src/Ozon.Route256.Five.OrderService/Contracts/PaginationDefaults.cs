namespace Ozon.Route256.Five.OrderService.Contracts;

public static class PaginationDefaults
{
    public const int PageNumber = 1;
    
    public const int PageSize = 10;

    public static int CheckPageNumber(int value) => value > 0 ? value : PageNumber;
    
    public static int CheckPageSize(int value) => value > 0 ? value : PageSize;
}