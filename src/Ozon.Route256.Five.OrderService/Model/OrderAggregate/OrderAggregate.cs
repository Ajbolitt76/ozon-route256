namespace Ozon.Route256.Five.OrderService.Model.OrderAggregate;

public record OrderAggregate(
    long Id,
    OrderState OrderState,
    CustomerModel Customer,
    List<OrderGood> Goods,
    DateTime OrderedAt,
    OrderType OrderType)
{
    public decimal TotalPrice { get; } = Goods.Sum(x => x.Price);
    
    public double TotalWeight { get; } = Goods.Sum(x => x.Weight); 
    
    public int ItemsCount { get; } = Goods.Count;
};