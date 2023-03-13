using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Five.OrderService.Contracts;
using Ozon.Route256.Five.OrderService.Contracts.GetAllOrdersForCustomer;
using Ozon.Route256.Five.OrderService.Contracts.GetForRegions;
using Ozon.Route256.Five.OrderService.Contracts.GetOrders;
using Ozon.Route256.Five.OrderService.Contracts.GetStatus;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;
using Ozon.Route256.Five.OrderService.Features.CancelOrder;
using Ozon.Route256.Five.OrderService.Features.GetAllOrders;
using Ozon.Route256.Five.OrderService.Features.GetForRegions;
using Ozon.Route256.Five.OrderService.Features.GetOrdersForCustomer;
using Ozon.Route256.Five.OrderService.Features.GetOrderStatus;

namespace Ozon.Route256.Five.OrderService.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ICommandDispatcher _commandDispatcher;

    public OrdersController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
    {
        _queryDispatcher = queryDispatcher;
        _commandDispatcher = commandDispatcher;
    }

    /// <summary>
    /// Ручка получения статуса заказов
    /// </summary>
    /// <param name="id">Id заказа</param>
    [HttpGet("{id:long}/Status")]
    [ProducesResponseType(typeof(GetStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDescription), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatus(long id, CancellationToken token)
        => (await _queryDispatcher.Dispatch<GetOrderStatusQuery, GetStatusResponse>(new(id), token))
            .ToActionResult();

    /// <summary>
    /// Ручка отмены заказа
    /// </summary>
    /// <param name="id">Id заказа</param>
    [HttpPost("{id:long}/Cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDescription), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDescription), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelOrder(long id, CancellationToken token)
        => (await _commandDispatcher.Dispatch<CancelOrderCommand>(new(id), token))
            .ToActionResult();

    /// <summary>
    /// Ручка получения всех заказов
    /// </summary>
    /// <param name="request">Запрос</param>
    [HttpGet("")]
    [ProducesResponseType(typeof(GetOrdersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDescription), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllOrders([FromQuery] GetOrdersRequest request, CancellationToken token)
        => (await _queryDispatcher.Dispatch<GetAllOrdersQuery, GetOrdersResponse>(
                new(
                    request.Regions,
                    request.IsAscending,
                    request.PageNumber,
                    request.PageSize),
                token))
            .ToActionResult();

    /// <summary>
    /// Ручка аггрегации закзов по регионам
    /// </summary>
    [HttpGet("AggregateForRegions")]
    [ProducesResponseType(typeof(GetForRegionsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDescription), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetForRegions([FromQuery] GetForRegionsRequest request, CancellationToken token)
        => (await _queryDispatcher.Dispatch<GetForRegionsQuery, GetForRegionsResponse>(
            new(
                request.StartFrom,
                request.Regions),
            token)).ToActionResult();

    /// <summary>
    /// Ручка получения заказов клиента
    /// </summary>
    [HttpGet("GetForCustomer")]
    [ProducesResponseType(typeof(GetAllOrdersForCustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDescription), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllOrdersForCustomer(
        [FromQuery] GetAllOrdersForCustomerRequest request,
        CancellationToken token)
        => (await _queryDispatcher.Dispatch<GetOrdersForCustomerQuery, GetAllOrdersForCustomerResponse>(
                new GetOrdersForCustomerQuery(
                    request.ClientId,
                    request.StartFrom,
                    request.PageNumber,
                    request.PageSize),
                token
            ))
            .ToActionResult();
}