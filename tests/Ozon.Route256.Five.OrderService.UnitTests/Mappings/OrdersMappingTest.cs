using FluentAssertions;
using Ozon.Route256.Five.OrderService.Mappings;
using Ozon.Route256.Five.OrderService.Model;
using OrderStateGrpc = Ozon.Route256.Five.OrdersService.Grpc.OrderState;

namespace Ozon.Route256.Five.OrderService.UnitTests.Mappings;

public class OrdersMappingTest
{
    /// <summary>
    /// Маппинг должен обрабатывать все варианты grpc контракта
    /// </summary>
    [Fact]
    public void Map_OrderState_ShouldCoverAllGrpcVariants()
    {
        var variants = Enum.GetValues<OrderState>();
        var knownMaps = new Dictionary<OrderState, OrderStateGrpc>()
        {
            [OrderState.Cancelled] = OrderStateGrpc.Cancelled,
            [OrderState.Created] = OrderStateGrpc.Created,
            [OrderState.Delivered] = OrderStateGrpc.Delivered,
            [OrderState.Lost] = OrderStateGrpc.Lost,
            [OrderState.SentToCustomer] = OrderStateGrpc.SentToCustomer
        };
        
        var mapped = variants.ToDictionary(x => x, x => x.ToOrderServiceDto());

        mapped.Should()
            .HaveSameCount(variants)
            .And
            .Equal(knownMaps);
    }    
}