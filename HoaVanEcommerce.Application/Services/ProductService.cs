using HoaVanEcommerce.BE.Application.DTOs.Products;
using HoaVanEcommerce.BE.Application.Interfaces;
using HoaVanEcommerce.Domain.Entities;

namespace HoaVanEcommerce.BE.Application.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<List<ProductListItemDto>> GetListAsync(int? categoryId, string? search, CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetListAsync(categoryId, search, cancellationToken);
        return products.Select(p => new ProductListItemDto
        {
            Id = p.Id,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name ?? string.Empty,
            Name = p.Name,
            Code = p.Code,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            IsActive = p.IsActive
        }).ToList();
    }

    public async Task<ProductDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var p = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (p is null) return null;

        return new ProductDetailDto
        {
            Id = p.Id,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name ?? string.Empty,
            Name = p.Name,
            Code = p.Code,
            Description = p.Description,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            StockQuantity = p.StockQuantity,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt
        };
    }

    public async Task<ProductDetailDto> CreateAsync(CreateOrUpdateProductRequest request, string imageUrl, CancellationToken cancellationToken = default)
    {
        if (!await _categoryRepository.ExistsAsync(request.CategoryId, cancellationToken))
        {
            throw new InvalidOperationException("CATEGORY_NOT_FOUND");
        }

        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            var existingCode = await _productRepository.GetByCodeAsync(request.Code.Trim(), cancellationToken);
            if (existingCode is not null)
            {
                throw new InvalidOperationException("PRODUCT_CODE_EXISTS");
            }
        }

        var product = new Product
        {
            CategoryId = request.CategoryId,
            Name = request.Name.Trim(),
            Code = string.IsNullOrWhiteSpace(request.Code) ? null : request.Code.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Price = request.Price,
            ImageUrl = imageUrl,
            StockQuantity = request.StockQuantity,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _productRepository.AddAsync(product, cancellationToken);
        // Re-load with category
        return (await GetByIdAsync(created.Id, cancellationToken))!;
    }

    public async Task<ProductDetailDto?> UpdateAsync(int id, CreateOrUpdateProductRequest request, string? newImageUrl, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null) return null;

        if (!await _categoryRepository.ExistsAsync(request.CategoryId, cancellationToken))
        {
            throw new InvalidOperationException("CATEGORY_NOT_FOUND");
        }

        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            var code = request.Code.Trim();
            var existingCode = await _productRepository.GetByCodeAsync(code, cancellationToken);
            if (existingCode is not null && existingCode.Id != id)
            {
                throw new InvalidOperationException("PRODUCT_CODE_EXISTS");
            }
            product.Code = code;
        }
        else
        {
            product.Code = null;
        }

        product.CategoryId = request.CategoryId;
        product.Name = request.Name.Trim();
        product.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;
        product.IsActive = request.IsActive;

        if (!string.IsNullOrWhiteSpace(newImageUrl))
        {
            product.ImageUrl = newImageUrl;
        }

        await _productRepository.UpdateAsync(product, cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null) return false;

        await _productRepository.DeleteAsync(product, cancellationToken);
        return true;
    }
}

