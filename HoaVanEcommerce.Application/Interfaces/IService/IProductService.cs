using HoaVanEcommerce.BE.Application.DTOs.Products;

namespace HoaVanEcommerce.BE.Application.Interfaces;

public interface IProductService
{
    // Public
    Task<List<ProductListItemDto>> GetListAsync(int? categoryId, string? search, CancellationToken cancellationToken = default);
    Task<ProductDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Admin
    Task<ProductDetailDto> CreateAsync(CreateOrUpdateProductRequest request, string imageUrl, CancellationToken cancellationToken = default);
    Task<ProductDetailDto?> UpdateAsync(int id, CreateOrUpdateProductRequest request, string? newImageUrl, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

