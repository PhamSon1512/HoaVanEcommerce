namespace HoaVanEcommerce.BE.Application.DTOs.Products;

public sealed class CreateOrUpdateProductRequest
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;
}

