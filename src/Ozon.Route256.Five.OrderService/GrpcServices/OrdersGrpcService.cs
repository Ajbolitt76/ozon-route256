using Grpc.Core;

namespace Ozon.Route256.Five.OrderService.GrpcServices;

public class OrdersGrpcService : Orders.OrdersBase
{
    public override Task<GetOrderByIdResponse> GetOrder(GetOrderByIdRequest request, ServerCallContext context)
    {
        throw new RpcException(new Status(StatusCode.NotFound, "Пользователь не найден"));
    }
}