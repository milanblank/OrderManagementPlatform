using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Interfaces;
using OrderManagement.Application.DTOs;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class StockController : ControllerBase
{
    private readonly IStockService _stockService;

    public StockController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpPut("{productId}/add-stock")]
    public async Task<IActionResult> AddStock(int productId, [FromBody] StockUpdateDto stockUpdate)
    {
        var success = await _stockService.AddStockAsync(productId, stockUpdate.Quantity);
        if (!success)
            return NotFound("Product not found");

        var newStock = await _stockService.GetCurrentStockAsync(productId);
        return Ok(new { ProductId = productId, NewStock = newStock });
    }

    [HttpPut("{productId}/remove-stock")]
    public async Task<IActionResult> RemoveStock(int productId, [FromBody] StockUpdateDto stockUpdate)
    {
        var success = await _stockService.RemoveStockAsync(productId, stockUpdate.Quantity);
        if (!success)
            return BadRequest("Product not found or insufficient stock");

        var newStock = await _stockService.GetCurrentStockAsync(productId);
        return Ok(new { ProductId = productId, NewStock = newStock });
    }
}

