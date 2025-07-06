using Microsoft.EntityFrameworkCore;
using OrderManagement.Infrastructure.Data;
using OrderManagement.Application.Interfaces;
using OrderManagement.Infrastructure.Repositories;
using OrderManagement.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Configure Entity Framework with LocalDB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

// Register Services
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IStockService, StockService>();

// Add JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Admin only policy (for stock management)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// Add controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Add a test endpoint to verify database connectivity
app.MapGet("/test-db", async (HttpContext httpContext) =>
{
    try
    {
        // Get the AppDbContext from dependency injection
        var context = httpContext.RequestServices.GetRequiredService<AppDbContext>();
        
        var canConnect = await context.Database.CanConnectAsync();
        var userCount = await context.Users.CountAsync();
        
        return Results.Ok(new { 
            CanConnect = canConnect, 
            UserCount = userCount,
            DatabaseName = context.Database.GetDbConnection().Database
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
})
.WithName("TestDatabase");

app.Run();


