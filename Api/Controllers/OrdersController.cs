using Api.SwaggerExamples;
using Application.Exceptions;
using Application.Orders;
using Client.Dtos;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderReader _orderReader;
    private readonly IOrderCreator _orderCreator;
    private readonly IOrderUpdater _orderUpdater;

    public OrdersController(IOrderReader orderReader,
                            IOrderCreator orderCreator,
                            IOrderUpdater orderUpdater)
    {
        _orderReader = orderReader;
        _orderCreator = orderCreator;
        _orderUpdater = orderUpdater;
    }

    [HttpGet("[action]")]
    public Task<IActionResult> Get([FromQuery]string orderNumber)
    {
        try
        {
            var response = _orderReader.ReadOrder(orderNumber);

            return Task.FromResult<IActionResult>(Ok(response));
        }
        // There aren't any data annotation attributes so will this ever catch a ValidationException?
        catch (ValidationException ex)
        {
            return Task.FromResult<IActionResult>(BadRequest(ex.Errors));
        }
        catch (Exception ex)
        {
            // Would a JSON response with the error be more appropriate Aas catching then throwing a generic exception doesn't seem right?
            // The consumer of the API could then use this response and redirect to a failure page or similar
            throw new Exception(ex.Message);
        }
    }   

    [HttpPost("[action]")]
    [SwaggerRequestExample(typeof(CreateOrderRequestDto), typeof(CreateOrderExample))]
    // should this be POST /orders ?
    public Task<IActionResult> Create([FromBody]CreateOrderRequestDto request)
    {
        try
        {
            var response = _orderCreator.CreateOrder(request);

            return Task.FromResult<IActionResult>(Ok(response));
        }
        catch (ValidationException ex)
        {
            return Task.FromResult<IActionResult>(BadRequest(ex.Errors));
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpPut("[action]")]
    [SwaggerRequestExample(typeof(UpdateOrderRequestDto), typeof(UpdateOrderExample))]
    public Task<IActionResult> Update([FromBody] UpdateOrderRequestDto request)
    {
        try
        {
            var response = _orderUpdater.UpdateOrder(request);

            return Task.FromResult<IActionResult>(Ok(response));
        }
        catch (ValidationException ex)
        {
            return Task.FromResult<IActionResult>(BadRequest(ex.Errors));
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
