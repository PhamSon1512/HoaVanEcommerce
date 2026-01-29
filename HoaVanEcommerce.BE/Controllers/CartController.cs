using HoaVanEcommerce.BE.Application.DTOs.Cart;
using HoaVanEcommerce.BE.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HoaVanEcommerce.BE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication for all cart endpoints
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
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

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var cart = await _cartService.GetCartAsync(userId, cancellationToken);
            return Ok(cart);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> AddToCart([FromBody] AddToCartRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = GetCurrentUserId();
            var cart = await _cartService.AddToCartAsync(userId, request, cancellationToken);
            return Ok(cart);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex) when (ex.Message == "PRODUCT_NOT_FOUND")
        {
            return NotFound(new { message = "Sản phẩm không tồn tại." });
        }
        catch (InvalidOperationException ex) when (ex.Message == "PRODUCT_NOT_AVAILABLE")
        {
            return BadRequest(new { message = "Sản phẩm không khả dụng." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("items/{cartItemId:int}")]
    public async Task<ActionResult<CartDto>> UpdateCartItem(int cartItemId, [FromBody] UpdateCartItemRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = GetCurrentUserId();
            var cart = await _cartService.UpdateCartItemAsync(userId, cartItemId, request, cancellationToken);
            return Ok(cart);
        }
        catch (UnauthorizedAccessException ex)
        {
            if (ex.Message == "CART_ACCESS_DENIED")
            {
                return Forbid();
            }
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex) when (ex.Message == "CART_ITEM_NOT_FOUND")
        {
            return NotFound(new { message = "Sản phẩm không tồn tại trong giỏ hàng." });
        }
    }

    [HttpDelete("items/{cartItemId:int}")]
    public async Task<ActionResult<CartDto>> RemoveCartItem(int cartItemId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var cart = await _cartService.RemoveCartItemAsync(userId, cartItemId, cancellationToken);
            return Ok(cart);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex) when (ex.Message == "CART_ITEM_NOT_FOUND")
        {
            return NotFound(new { message = "Sản phẩm không tồn tại trong giỏ hàng." });
        }
    }

    [HttpDelete("clear")]
    public async Task<ActionResult> ClearCart(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _cartService.ClearCartAsync(userId, cancellationToken);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
