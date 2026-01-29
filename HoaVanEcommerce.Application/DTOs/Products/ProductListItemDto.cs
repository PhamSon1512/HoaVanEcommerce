namespace HoaVanEcommerce.BE.Application.DTOs.Products;

public sealed class ProductListItemDto
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = null!;
    public bool IsActive { get; set; }
}

