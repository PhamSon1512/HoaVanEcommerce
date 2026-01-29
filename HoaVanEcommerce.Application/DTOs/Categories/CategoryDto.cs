namespace HoaVanEcommerce.BE.Application.DTOs.Categories;

public sealed class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
}

