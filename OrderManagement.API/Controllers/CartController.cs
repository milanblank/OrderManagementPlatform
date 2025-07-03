using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<CartDto>> GetCart(int userId)
    {
        try
        {
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("add")]
    public async Task<ActionResult<CartItemDto>> AddToCart(AddToCartDto dto)
    {
        try
        {
            var cartItem = await _cartService.AddToCartAsync(dto);
            return Ok(cartItem);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    [HttpPut("items/{cartItemId}")]
    public async Task<ActionResult<CartItemDto>> UpdateCartItem(int cartItemId, UpdateCartItemDto dto)
    {
        try
        {
            var cartItem = await _cartService.UpdateCartItemAsync(cartItemId, dto);
            return cartItem == null ? NoContent() : Ok(cartItem);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpDelete("items/{cartItemId}")]
    public async Task<IActionResult> RemoveFromCart(int cartItemId)
    {
        try
        {
            var result = await _cartService.RemoveFromCartAsync(cartItemId);
            return result ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> ClearCart(int userId)
    {
        try
        {
            await _cartService.ClearCartAsync(userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("{userId}/checkout")]
    public async Task<ActionResult<OrderDto>> PlaceOrder(int userId)
    {
        try
        {
            var order = await _cartService.PlaceOrderAsync(userId);
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }
}