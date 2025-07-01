namespace OrderManagement.Domain.Entities;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    // Navigation properties (optional, useful for ORM like EF Core)
    public Order Order { get; set; }
    public Product Product { get; set; }
}