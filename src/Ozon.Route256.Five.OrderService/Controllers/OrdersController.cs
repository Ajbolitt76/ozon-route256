using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Five.OrderService.Contracts;
using Ozon.Route256.Five.OrderService.Contracts.GetAllOrdersForClient;
using Ozon.Route256.Five.OrderService.Contracts.GetForRegions;
using Ozon.Route256.Five.OrderService.Contracts.GetOrders;
using Ozon.Route256.Five.OrderService.Contracts.GetStatus;

namespace Ozon.Route256.Five.OrderService.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    //2.2
    [HttpGet("{id:long}/Status")]
    [ProducesResponseType(typeof(GetStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDescription), StatusCodes.Status404NotFound)]
    public IActionResult GetStatus(long id)
    {
        //TODO: Бизнес логика, в следующих заданиях
        return NotFound(new ErrorDescription("ORDER_NOT_FOUND", "Ваш заказ не был найден"));
    }

    //2.1
    [HttpPost("{id:long}/Cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDescription), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDescription), StatusCodes.Status404NotFound)]
    public IActionResult CancelOrder(long id)
    {
        //TODO: Бизнес логика, в следующих заданиях
        return NotFound(new ErrorDescription("ORDER_NOT_FOUND", "Ваш заказ не был найден"));
    }
    
    //2.5
    [HttpGet("")]
    [ProducesResponseType(typeof(GetOrdersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDescription), StatusCodes.Status400BadRequest)]
    public IActionResult GetAllOrders([FromQuery]GetOrdersRequest request)
    {
        //TODO: Бизнес логика, в следующих заданиях
        return Ok(new GetOrdersResponse(new List<GetOrdersOrderResponseItem>()));
    }

    //2.6
    [HttpGet("AggregateForRegions")]
    [ProducesResponseType(typeof(GetForRegionsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDescription), StatusCodes.Status400BadRequest)]
    public IActionResult GetForRegions([FromQuery] GetForRegionsRequest request)
    {
        //TODO: Бизнес логика, в следующих заданиях
        return Ok(new GetForRegionsResponse(new List<GetForRegionsResponseItem>()));
    }
    
    //2.7
    [HttpGet("GetForCustomer")]
    [ProducesResponseType(typeof(GetAllOrdersForClientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDescription), StatusCodes.Status404NotFound)]
    public IActionResult GetAllOrdersForCustomer([FromQuery]GetAllOrdersForClientRequest request)
    {
        //TODO: Бизнес логика, в следующих заданиях
        return NotFound(new ErrorDescription("CUSTOMER_NOT_FOUND", "Пользователь не найден"));
    }
}