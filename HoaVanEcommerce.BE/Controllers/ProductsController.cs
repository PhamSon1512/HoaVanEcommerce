using HoaVanEcommerce.BE.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HoaVanEcommerce.BE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // Public list, supports filtering/search
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int? categoryId, [FromQuery] string? search, CancellationToken cancellationToken)
    {
        var products = await _productService.GetListAsync(categoryId, search, cancellationToken);
        return Ok(products);
    }

    // Public detail
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        if (product == null) return NotFound();
        return Ok(product);
    }
}

