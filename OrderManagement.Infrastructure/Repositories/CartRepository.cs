using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;
using OrderManagement.Infrastructure.Data;

namespace OrderManagement.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CartDto> GetCartByUserIdAsync(int userId)
    {
        var cartItems = await _context.CartItems
            .Where(ci => ci.UserId == userId && ci.OrderId == null)
            .Include(ci => ci.Product)
            .ToListAsync();

        return new CartDto
        {
            UserId = userId,
            Items = cartItems.Select(MapToCartItemDto).ToList()
        };
    }

    public async Task<CartItemDto> AddToCartAsync(AddToCartDto dto)
    {
        // Check if item already exists in cart
        var existingItem = await _context.CartItems
            .Include(ci => ci.Product)
            .FirstOrDefaultAsync(ci => ci.UserId == dto.UserId && 
                                     ci.ProductId == dto.ProductId && 
                                     ci.OrderId == null);

        if (existingItem != null)
        {
            // Update quantity if item already exists
            existingItem.Quantity += dto.Quantity;
            await _context.SaveChangesAsync();
            return MapToCartItemDto(existingItem);
        }

        // Create new cart item
        var cartItem = new CartItem
        {
            UserId = dto.UserId,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity
        };

        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();

        // Reload with product information
        await _context.Entry(cartItem)
            .Reference(ci => ci.Product)
            .LoadAsync();

        return MapToCartItemDto(cartItem);
    }

    public async Task<CartItemDto?> UpdateCartItemAsync(int cartItemId, UpdateCartItemDto dto)
    {
        var cartItem = await _context.CartItems
            .Include(ci => ci.Product)
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.OrderId == null);

        if (cartItem == null) return null;

        cartItem.Quantity = dto.Quantity;
        await _context.SaveChangesAsync();

        return MapToCartItemDto(cartItem);
    }

    public async Task<bool> RemoveFromCartAsync(int cartItemId)
    {
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.OrderId == null);

        if (cartItem == null) return false;

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ClearCartAsync(int userId)
    {
        var cartItems = _context.CartItems
            .Where(ci => ci.UserId == userId && ci.OrderId == null);

        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CartItemExistsAsync(int userId, int productId)
    {
        return await _context.CartItems
            .AnyAsync(ci => ci.UserId == userId && 
                           ci.ProductId == productId && 
                           ci.OrderId == null);
    }

    public async Task<CartItemDto?> GetCartItemAsync(int userId, int productId)
    {
        var cartItem = await _context.CartItems
            .Include(ci => ci.Product)
            .FirstOrDefaultAsync(ci => ci.UserId == userId && 
                                     ci.ProductId == productId && 
                                     ci.OrderId == null);

        return cartItem == null ? null : MapToCartItemDto(cartItem);
    }

    private static CartItemDto MapToCartItemDto(CartItem cartItem)
    {
        return new CartItemDto
        {
            Id = cartItem.Id,
            ProductId = cartItem.ProductId,
            ProductName = cartItem.Product?.Name ?? "",
            Quantity = cartItem.Quantity,
            UnitPrice = cartItem.Product?.Price ?? 0
        };
    }
}