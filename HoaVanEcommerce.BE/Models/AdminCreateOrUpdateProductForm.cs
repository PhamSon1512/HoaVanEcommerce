using Microsoft.AspNetCore.Http;

namespace HoaVanEcommerce.BE.Models;

/// <summary>
/// Web-only DTO used to bind multipart/form-data for admin product create/update.
/// Keeps IFormFile at the API layer while mapping to application DTOs internally.
/// </summary>
public sealed class AdminCreateOrUpdateProductForm
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Optional image file uploaded with the product.
    /// </summary>
    public IFormFile? Image { get; set; }
}

