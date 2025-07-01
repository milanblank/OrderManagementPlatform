using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Infrastructure.Data;

namespace OrderManagement.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return null;

        var items = await _context.OrderItems
            .Where(oi => oi.OrderId == order.Id)
            .Include(oi => oi.Product)
            .ToListAsync();

        return MapToDto(order, items);
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync()
    {
        var orders = await _context.Orders.ToListAsync();
        var orderItems = await _context.OrderItems.Include(oi => oi.Product).ToListAsync();

        return orders.Select(order =>
        {
            var items = orderItems.Where(oi => oi.OrderId == order.Id).ToList();
            return MapToDto(order, items);
        });
    }

    public async Task<IEnumerable<OrderDto>> GetByUserIdAsync(int userId)
    {
        var orders = await _context.Orders.Where(o => o.UserId == userId).ToListAsync();
        var orderItems = await _context.OrderItems.Include(oi => oi.Product).ToListAsync();

        return orders.Select(order =>
        {
            var items = orderItems.Where(oi => oi.OrderId == order.Id).ToList();
            return MapToDto(order, items);
        });
    }

    public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
    {
        var order = new Order
        {
            UserId = dto.UserId,
            TotalPrice = 0, // Will be calculated
            Status = OrderStatus.Pending,
            Date = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        decimal total = 0;
        var orderItems = new List<OrderItem>();

        foreach (var item in dto.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product == null) continue;

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            };
            total += product.Price * item.Quantity;
            orderItems.Add(orderItem);
        }

        _context.OrderItems.AddRange(orderItems);
        order.TotalPrice = total;
        await _context.SaveChangesAsync();

        return MapToDto(order, orderItems);
    }

    public async Task<OrderDto?> UpdateStatusAsync(int id, string status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return null;

        if (Enum.TryParse<OrderStatus>(status, out var newStatus))
        {
            order.Status = newStatus;
            await _context.SaveChangesAsync();
        }

        var items = await _context.OrderItems
            .Where(oi => oi.OrderId == order.Id)
            .Include(oi => oi.Product)
            .ToListAsync();

        return MapToDto(order, items);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return false;

        var items = _context.OrderItems.Where(oi => oi.OrderId == id);
        _context.OrderItems.RemoveRange(items);
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Orders.AnyAsync(o => o.Id == id);
    }

    private OrderDto MapToDto(Order order, List<OrderItem> items)
    {
        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            UserName = "", // Optionally fetch user name if needed
            TotalPrice = order.TotalPrice,
            Status = order.Status.ToString(),
            Date = order.Date,
            Items = items.Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                ProductName = oi.Product?.Name ?? "",
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList()
        };
    }
}