using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Five.OrderService.Contracts.GetAllCustomers;
using Ozon.Route256.Five.OrderService.Contracts.GetRegions;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;
using Ozon.Route256.Five.OrderService.Features.GetAllCustomers;
using Ozon.Route256.Five.OrderService.Features.GetRegions;

namespace Ozon.Route256.Five.OrderService.Controllers;

[ApiController]
[Route("[controller]/")]
public class DataController : ControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;

    public DataController(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    /// <summary>
    /// Ручка получения списка клиентов
    /// </summary>
    [HttpGet("Customers")]
    [ProducesResponseType(typeof(GetAllCustomersResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCustomers(CancellationToken cancellationToken)
        => (await _queryDispatcher.Dispatch<GetAllCustomerQuery, GetAllCustomersResponse>(
                new GetAllCustomerQuery(),
                cancellationToken))
            .ToActionResult();

    /// <summary>
    /// Ручка получения регионов
    /// </summary>
    [HttpGet("Regions")]
    [ProducesResponseType(typeof(GetRegionsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRegions(CancellationToken cancellationToken)
        => (await _queryDispatcher.Dispatch<GetRegionsQuery, GetRegionsResponse>(
                new(), cancellationToken))
            .ToActionResult();
}