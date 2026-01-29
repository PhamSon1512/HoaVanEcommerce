using HoaVanEcommerce.BE.Application.Interfaces;
using HoaVanEcommerce.Domain.Entities;
using HoaVanEcommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HoaVanEcommerce.Infrastructure.Repositories;

public sealed class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CartRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Cart?> GetActiveCartByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == "ACTIVE", cancellationToken);
    }

    public async Task<Cart> CreateCartAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        await _dbContext.Carts.AddAsync(cart, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return cart;
    }

    public Task UpdateCartAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        _dbContext.Carts.Update(cart);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<CartItem?> GetCartItemByIdAsync(int cartItemId, CancellationToken cancellationToken = default)
    {
        return _dbContext.CartItems
            .Include(i => i.Product)
                .ThenInclude(p => p.Category)
            .Include(i => i.Cart)
            .FirstOrDefaultAsync(i => i.Id == cartItemId, cancellationToken);
    }

    public Task<CartItem?> GetCartItemByCartAndProductAsync(int cartId, int productId, CancellationToken cancellationToken = default)
    {
        return _dbContext.CartItems
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.CartId == cartId && i.ProductId == productId, cancellationToken);
    }

    public async Task<CartItem> AddCartItemAsync(CartItem item, CancellationToken cancellationToken = default)
    {
        await _dbContext.CartItems.AddAsync(item, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return item;
    }

    public Task UpdateCartItemAsync(CartItem item, CancellationToken cancellationToken = default)
    {
        _dbContext.CartItems.Update(item);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteCartItemAsync(CartItem item, CancellationToken cancellationToken = default)
    {
        _dbContext.CartItems.Remove(item);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ClearCartItemsAsync(int cartId, CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.CartItems
            .Where(i => i.CartId == cartId)
            .ToListAsync(cancellationToken);
        
        _dbContext.CartItems.RemoveRange(items);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
