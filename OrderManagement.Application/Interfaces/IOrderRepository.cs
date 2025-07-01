using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Interfaces;

public interface IOrderRepository
{
    Task<OrderDto?> GetByIdAsync(int id);
    Task<IEnumerable<OrderDto>> GetAllAsync();
    Task<IEnumerable<OrderDto>> GetByUserIdAsync(int userId);
    Task<OrderDto> CreateAsync(CreateOrderDto dto);
    Task<OrderDto?> UpdateStatusAsync(int id, string status);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}