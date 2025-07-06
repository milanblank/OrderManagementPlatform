using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;

namespace OrderManagement.Application.Services;

public class StockService : IStockService
{
    private readonly IProductRepository _productRepository;

    public StockService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<bool> AddStockAsync(int productId, int quantity)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null) return false;

        var updateDto = new UpdateProductDto
        {
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity + quantity,
            Category = product.Category
        };

        await _productRepository.UpdateAsync(productId, updateDto);
        return true;
    }

    public async Task<bool> RemoveStockAsync(int productId, int quantity)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null || product.StockQuantity < quantity) return false;

        var updateDto = new UpdateProductDto
        {
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity - quantity,
            Category = product.Category
        };

        await _productRepository.UpdateAsync(productId, updateDto);
        return true;
    }

    public async Task<int?> GetCurrentStockAsync(int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        return product?.StockQuantity;
    }
}