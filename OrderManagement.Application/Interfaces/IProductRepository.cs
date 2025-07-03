using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Interfaces;

public interface IProductRepository
{
    Task<ProductDto?> GetByIdAsync(int id);
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<IEnumerable<ProductDto>> GetByCategoryAsync(string category);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> IsInStockAsync(int productId, int quantity);
    Task<bool> ReduceStockAsync(int productId, int quantity);
    Task<bool> RestoreStockAsync(int productId, int quantity);
}