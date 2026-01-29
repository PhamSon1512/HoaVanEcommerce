using HoaVanEcommerce.Domain.Entities;

namespace HoaVanEcommerce.BE.Application.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<List<Product>> GetListAsync(int? categoryId, string? search, CancellationToken cancellationToken = default);
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task DeleteAsync(Product product, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}

