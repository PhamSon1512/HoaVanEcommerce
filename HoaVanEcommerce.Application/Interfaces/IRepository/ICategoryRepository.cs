using HoaVanEcommerce.Domain.Entities;

namespace HoaVanEcommerce.BE.Application.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}

