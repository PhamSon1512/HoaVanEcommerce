using HoaVanEcommerce.BE.Application.DTOs.Cart;

namespace HoaVanEcommerce.BE.Application.Interfaces;

public interface ICartService
{
    Task<CartDto> GetCartAsync(int userId, CancellationToken cancellationToken = default);
    Task<CartDto> AddToCartAsync(int userId, AddToCartRequest request, CancellationToken cancellationToken = default);
    Task<CartDto> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemRequest request, CancellationToken cancellationToken = default);
    Task<CartDto> RemoveCartItemAsync(int userId, int cartItemId, CancellationToken cancellationToken = default);
    Task ClearCartAsync(int userId, CancellationToken cancellationToken = default);
}
