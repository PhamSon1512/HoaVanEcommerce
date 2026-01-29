using HoaVanEcommerce.BE.Application.DTOs.Categories;
using HoaVanEcommerce.BE.Application.Interfaces;

namespace HoaVanEcommerce.BE.Application.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Slug = c.Slug ?? string.Empty,
            Description = c.Description,
            ThumbnailUrl = null
        }).ToList();
    }
}

