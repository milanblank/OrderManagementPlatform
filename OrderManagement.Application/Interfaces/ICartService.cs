using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Interfaces;

public interface ICartService
{
    Task<CartDto> GetCartAsync(int userId);
    Task<CartItemDto> AddToCartAsync(AddToCartDto dto);
    Task<CartItemDto?> UpdateCartItemAsync(int cartItemId, UpdateCartItemDto dto);
    Task<bool> RemoveFromCartAsync(int cartItemId);
    Task<bool> ClearCartAsync(int userId);
    Task<OrderDto> PlaceOrderAsync(int userId);
}