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

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int? categoryId, [FromQuery] string? search)
    {
        var products = await _productService.GetListAsync(categoryId, search);
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }
}

