using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Five.OrderService.Contracts.GetAllCustomers;
using Ozon.Route256.Five.OrderService.Contracts.GetRegions;

namespace Ozon.Route256.Five.OrderService.Controllers;

[ApiController]
[Route("[controller]/")]
public class DataController : ControllerBase
{
    /// <summary>
    /// Ручка получения списка клиентов
    /// </summary>
    [HttpGet("Customers")]
    [ProducesResponseType(typeof(GetAllCustomersResponse), StatusCodes.Status200OK)]
    public IActionResult GetCustomers()
    {
        //TODO: Бизнес логика, в следующих заданиях
        return Ok(new GetAllCustomersResponse(new List<GetAllCustomersResponseItem>()));
    }
    
    /// <summary>
    /// Ручка получения регионов
    /// </summary>
    [HttpGet("Regions")]
    [ProducesResponseType(typeof(GetRegionsResponse), StatusCodes.Status200OK)]
    public IActionResult GetRegions()
    {
        //TODO: Бизнес логика, в следующих заданиях
        return Ok(new GetRegionsResponse(new List<string>()));
    }
}