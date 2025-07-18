namespace OrderManagement.Domain.Entities;

public class CartItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int? OrderId { get; set; }

    // Navigation properties (optional, useful for ORM like EF Core)
    public User User { get; set; }
    public Product Product { get; set; }
    public Order? Order { get; set; }
}