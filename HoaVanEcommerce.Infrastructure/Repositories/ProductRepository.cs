using HoaVanEcommerce.BE.Application.Interfaces;
using HoaVanEcommerce.Domain.Entities;
using HoaVanEcommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HoaVanEcommerce.Infrastructure.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProductRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return _dbContext.Products
            .FirstOrDefaultAsync(p => p.Code != null && p.Code == code, cancellationToken);
    }

    public Task<List<Product>> GetListAsync(int? categoryId, string? search, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Products
            .Include(p => p.Category)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(s) ||
                (p.Code != null && p.Code.ToLower().Contains(s)));
        }

        return query
            .OrderByDescending(p => p.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _dbContext.Products.AddAsync(product, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return product;
    }

    public Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _dbContext.Products.Update(product);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
    {
        // Xóa tất cả cart_items liên quan đến sản phẩm này trước
        var cartItems = await _dbContext.CartItems
            .Where(ci => ci.ProductId == product.Id)
            .ToListAsync(cancellationToken);
        
        if (cartItems.Any())
        {
            _dbContext.CartItems.RemoveRange(cartItems);
        }

        // Xóa tất cả order_items liên quan đến sản phẩm này
        // Lưu ý: Điều này sẽ xóa lịch sử đơn hàng liên quan đến sản phẩm
        // Nếu muốn giữ lịch sử, cần migration để cho phép ProductId nullable trong order_items
        var orderItems = await _dbContext.OrderItems
            .Where(oi => oi.ProductId == product.Id)
            .ToListAsync(cancellationToken);
        
        if (orderItems.Any())
        {
            _dbContext.OrderItems.RemoveRange(orderItems);
        }

        // Xóa sản phẩm
        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Products.AnyAsync(p => p.Id == id, cancellationToken);
    }
}

