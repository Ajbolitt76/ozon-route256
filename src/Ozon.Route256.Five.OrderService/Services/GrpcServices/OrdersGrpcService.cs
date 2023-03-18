using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Exceptions;
using Ozon.Route256.Five.OrderService.Features.GetOrderById;
using Ozon.Route256.Five.OrderService.Mappings;
using Ozon.Route256.Five.OrdersService.Grpc;

namespace Ozon.Route256.Five.OrderService.Services.GrpcServices;

public class OrdersGrpcService : Orders.OrdersBase
{
    private readonly IQueryDispatcher _queryDispatcher;

    public OrdersGrpcService(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    /// <summary>
    /// Ручка получения заказа
    /// </summary>
    public override async Task<GetOrderByIdResponse> GetOrder(GetOrderByIdRequest request, ServerCallContext context)
    {
        var result = await _queryDispatcher.Dispatch<GetOrderByIdQuery, 
            Ozon.Route256.Five.OrderService.Contracts.GetOrderById.GetOrderByIdResponse>(
            new GetOrderByIdQuery(request.Id),
            context.CancellationToken);

        if (!result.Success)
            throw result.Error.ToRpcException();

        var response = result.Value!;

        return new GetOrderByIdResponse()
        {
            Id = response.Id,
            OrderState = response.OrderState.ToOrderServiceDto(),
            ItemsCount = (uint)response.ItemsCount,
            OrderedAt = Timestamp.FromDateTime(response.OrderedAt.ToUniversalTime()),
            ClientName = response.ClientName,
            ShippingAddress = response.ShippingAddress.ToOrderServiceDto(),
            Phone = response.Phone,
        };
    }
}