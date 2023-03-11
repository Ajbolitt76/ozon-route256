namespace Ozon.Route256.Five.OrderService.Model.OrderAggregate;

public record OrderAggregate(
    int Id,
    OrderState OrderState,
    CustomerDto Customer,
    List<OrderGood> Goods,
    DateTime OrderedAt,
    string OrderType)
{
    public decimal TotalPrice { get; } = Goods.Sum(x => x.Price);
    
    public double TotalWeight { get; } = Goods.Sum(x => x.Weight); 
    
    public int ItemsCount { get; } = Goods.Count;
};