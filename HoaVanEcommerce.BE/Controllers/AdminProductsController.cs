using HoaVanEcommerce.BE.Application.DTOs.Products;
using HoaVanEcommerce.BE.Application.Interfaces;
using HoaVanEcommerce.BE.Models;
using Microsoft.AspNetCore.Mvc;

namespace HoaVanEcommerce.BE.Controllers;

[ApiController]
[Route("api/admin/products")]
public class AdminProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IImageStorageService _imageStorage;

    public AdminProductsController(IProductService productService, IImageStorageService imageStorage)
    {
        _productService = productService;
        _imageStorage = imageStorage;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int? categoryId, [FromQuery] string? search, CancellationToken cancellationToken)
    {
        var products = await _productService.GetListAsync(categoryId, search, cancellationToken);
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        if (product == null) return NotFound();
        return Ok(product);
    }

    // multipart/form-data: fields + file (wrapped in a web-only DTO to satisfy Swagger)
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] AdminCreateOrUpdateProductForm form, CancellationToken cancellationToken)
    {
        if (form.Image is null)
        {
            return BadRequest(new { message = "Image file is required." });
        }

        var request = new CreateOrUpdateProductRequest
        {
            CategoryId = form.CategoryId,
            Name = form.Name,
            Code = form.Code,
            Description = form.Description,
            Price = form.Price,
            StockQuantity = form.StockQuantity,
            IsActive = form.IsActive
        };

        await using var stream = form.Image.OpenReadStream();
        var imageUrl = await _imageStorage.UploadProductImageAsync(stream, form.Image.FileName, form.Image.ContentType, cancellationToken);
        var created = await _productService.CreateAsync(request, imageUrl, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // multipart/form-data: fields + optional file (wrapped in a web-only DTO)
    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(int id, [FromForm] AdminCreateOrUpdateProductForm form, CancellationToken cancellationToken)
    {
        var request = new CreateOrUpdateProductRequest
        {
            CategoryId = form.CategoryId,
            Name = form.Name,
            Code = form.Code,
            Description = form.Description,
            Price = form.Price,
            StockQuantity = form.StockQuantity,
            IsActive = form.IsActive
        };

        string? newImageUrl = null;
        if (form.Image is not null)
        {
            await using var stream = form.Image.OpenReadStream();
            newImageUrl = await _imageStorage.UploadProductImageAsync(stream, form.Image.FileName, form.Image.ContentType, cancellationToken);
        }

        var updated = await _productService.UpdateAsync(id, request, newImageUrl, cancellationToken);
        if (updated is null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var ok = await _productService.DeleteAsync(id, cancellationToken);
        if (!ok) return NotFound();
        return NoContent();
    }
}

