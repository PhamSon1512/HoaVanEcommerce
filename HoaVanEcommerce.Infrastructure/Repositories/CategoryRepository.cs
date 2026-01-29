using HoaVanEcommerce.BE.Application.Interfaces;
using HoaVanEcommerce.Domain.Entities;
using HoaVanEcommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HoaVanEcommerce.Infrastructure.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CategoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return _dbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Categories.AnyAsync(c => c.Id == id, cancellationToken);
    }
}

