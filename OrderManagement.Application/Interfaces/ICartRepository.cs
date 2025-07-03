using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Interfaces;

public interface ICartRepository
{
    Task<CartDto> GetCartByUserIdAsync(int userId);
    Task<CartItemDto> AddToCartAsync(AddToCartDto dto);
    Task<CartItemDto?> UpdateCartItemAsync(int cartItemId, UpdateCartItemDto dto);
    Task<bool> RemoveFromCartAsync(int cartItemId);
    Task<bool> ClearCartAsync(int userId);
    Task<bool> CartItemExistsAsync(int userId, int productId);
    Task<CartItemDto?> GetCartItemAsync(int userId, int productId);
}