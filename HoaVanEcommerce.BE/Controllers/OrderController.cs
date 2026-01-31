using HoaVanEcommerce.BE.Application.DTOs.Orders;
using HoaVanEcommerce.BE.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HoaVanEcommerce.BE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }
        return userId;
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = GetCurrentUserId();
            var order = await _orderService.CreateOrderAsync(userId, request, cancellationToken);
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var order = await _orderService.GetOrderByIdAsync(id, userId, cancellationToken);
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpGet("my-orders")]
    public async Task<ActionResult<List<OrderDto>>> GetMyOrders(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var orders = await _orderService.GetUserOrdersAsync(userId, cancellationToken);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("all")]
    [Authorize] // TODO: Add admin role check
    public async Task<ActionResult<List<OrderDto>>> GetAllOrders(CancellationToken cancellationToken)
    {
        try
        {
            var orders = await _orderService.GetAllOrdersAsync(cancellationToken);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:int}/confirm-payment")]
    public async Task<ActionResult<OrderDto>> ConfirmPayment(int id, [FromBody] ConfirmPaymentRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = GetCurrentUserId();
            var order = await _orderService.ConfirmPaymentAsync(id, userId, request, cancellationToken);
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/status")]
    [Authorize] // TODO: Add admin role check
    public async Task<ActionResult<OrderDto>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var order = await _orderService.UpdateOrderStatusAsync(id, request, cancellationToken);
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:int}/confirm-received")]
    public async Task<ActionResult<OrderDto>> ConfirmReceived(int id, [FromBody] ConfirmReceivedRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = GetCurrentUserId();
            var order = await _orderService.ConfirmReceivedAsync(id, userId, request, cancellationToken);
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
