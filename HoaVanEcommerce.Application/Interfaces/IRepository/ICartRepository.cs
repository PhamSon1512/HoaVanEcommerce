using HoaVanEcommerce.Domain.Entities;

namespace HoaVanEcommerce.BE.Application.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetActiveCartByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Cart> CreateCartAsync(Cart cart, CancellationToken cancellationToken = default);
    Task UpdateCartAsync(Cart cart, CancellationToken cancellationToken = default);
    Task<CartItem?> GetCartItemByIdAsync(int cartItemId, CancellationToken cancellationToken = default);
    Task<List<CartItem>> GetCartItemsByIdsAsync(List<int> cartItemIds, CancellationToken cancellationToken = default);
    Task<CartItem?> GetCartItemByCartAndProductAsync(int cartId, int productId, CancellationToken cancellationToken = default);
    Task<CartItem> AddCartItemAsync(CartItem item, CancellationToken cancellationToken = default);
    Task UpdateCartItemAsync(CartItem item, CancellationToken cancellationToken = default);
    Task DeleteCartItemAsync(CartItem item, CancellationToken cancellationToken = default);
    Task ClearCartItemsAsync(int cartId, CancellationToken cancellationToken = default);
}
