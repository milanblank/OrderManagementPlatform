namespace OrderManagement.Application.Interfaces;

public interface IStockService
{
    Task<bool> AddStockAsync(int productId, int quantity);
    Task<bool> RemoveStockAsync(int productId, int quantity);
    Task<int?> GetCurrentStockAsync(int productId);
}