using OrderManagement.Application.DTOs;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Factories;

public interface IEntityFactory
{
    Product CreateProduct(CreateProductDto dto);
    User CreateUser(CreateUserDto dto);
    Order CreateOrder(CreateOrderDto dto);
    bool VerifyPassword(string password, string hash);
}

public class EntityFactory : IEntityFactory
{
    public Product CreateProduct(CreateProductDto dto)
    {
        return new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            Category = dto.Category
        };
    }

    public User CreateUser(CreateUserDto dto)
    {
        return new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),
            Role = Enum.Parse<Role>(dto.Role)
        };
    }

    public Order CreateOrder(CreateOrderDto dto)
    {
        return new Order
        {
            UserId = dto.UserId,
            TotalPrice = 0,
            Status = OrderStatus.Pending,
            Date = DateTime.UtcNow
        };
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, 12); // 12 is the work factor
    }
}