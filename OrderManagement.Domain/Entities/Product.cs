namespace OrderManagement.Domain.Entities;

public class Product
{
    public int Id { get; set; } // primary key
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string Category { get; set; } = string.Empty;
}