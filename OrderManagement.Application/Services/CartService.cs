using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;

namespace OrderManagement.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    public CartService(
        ICartRepository cartRepository,
        IProductRepository productRepository,
        IOrderRepository orderRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }

    public async Task<CartDto> GetCartAsync(int userId)
    {
        return await _cartRepository.GetCartByUserIdAsync(userId);
    }

    public async Task<CartItemDto> AddToCartAsync(AddToCartDto dto)
    {
        // Validate product exists and is in stock
        var isInStock = await _productRepository.IsInStockAsync(dto.ProductId, dto.Quantity);
        if (!isInStock)
        {
            throw new InvalidOperationException($"Product {dto.ProductId} is not available in the requested quantity.");
        }

        return await _cartRepository.AddToCartAsync(dto);
    }

    public async Task<CartItemDto?> UpdateCartItemAsync(int cartItemId, UpdateCartItemDto dto)
    {
        if (dto.Quantity <= 0)
        {
            await _cartRepository.RemoveFromCartAsync(cartItemId);
            return null;
        }

        return await _cartRepository.UpdateCartItemAsync(cartItemId, dto);
    }

    public async Task<bool> RemoveFromCartAsync(int cartItemId)
    {
        return await _cartRepository.RemoveFromCartAsync(cartItemId);
    }

    public async Task<bool> ClearCartAsync(int userId)
    {
        return await _cartRepository.ClearCartAsync(userId);
    }

    public async Task<OrderDto> PlaceOrderAsync(int userId)
    {
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);
        
        if (!cart.Items.Any())
        {
            throw new InvalidOperationException("Cannot place order with empty cart.");
        }

        // Validate stock availability for all items
        foreach (var item in cart.Items)
        {
            var isInStock = await _productRepository.IsInStockAsync(item.ProductId, item.Quantity);
            if (!isInStock)
            {
                throw new InvalidOperationException($"Product '{item.ProductName}' is not available in the requested quantity.");
            }
        }

        // Create order from cart items
        var createOrderDto = new CreateOrderDto
        {
            UserId = userId,
            Items = cart.Items.Select(item => new CreateOrderItemDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList()
        };

        var order = await _orderRepository.CreateAsync(createOrderDto);

        // Clear the cart after successful order placement
        await _cartRepository.ClearCartAsync(userId);

        return order;
    }
}