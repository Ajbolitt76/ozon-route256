using Grpc.Core;
using Ozon.Route256.Five.OrdersService.Grpc;

namespace Ozon.Route256.Five.OrderService.GrpcServices;

public class OrdersGrpcService : Orders.OrdersBase
{
    /// <summary>
    /// Ручка получения заказа
    /// </summary>
    public override Task<GetOrderByIdResponse> GetOrder(GetOrderByIdRequest request, ServerCallContext context)
    {
        throw new RpcException(new Status(StatusCode.NotFound, "Пользователь не найден"));
    }
}