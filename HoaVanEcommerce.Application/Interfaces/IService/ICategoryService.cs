using HoaVanEcommerce.BE.Application.DTOs.Categories;

namespace HoaVanEcommerce.BE.Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
}

