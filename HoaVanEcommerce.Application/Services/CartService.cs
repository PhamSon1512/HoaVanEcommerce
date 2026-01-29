using HoaVanEcommerce.BE.Application.DTOs.Cart;
using HoaVanEcommerce.BE.Application.Interfaces;
using HoaVanEcommerce.Domain.Entities;

namespace HoaVanEcommerce.BE.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartService(ICartRepository cartRepository, IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<CartDto> GetCartAsync(int userId, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetActiveCartByUserIdAsync(userId, cancellationToken);
        
        if (cart == null)
        {
            // Create new cart if doesn't exist
            cart = new Cart
            {
                UserId = userId,
                Status = "ACTIVE",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            cart = await _cartRepository.CreateCartAsync(cart, cancellationToken);
        }

        return MapToDto(cart);
    }

    public async Task<CartDto> AddToCartAsync(int userId, AddToCartRequest request, CancellationToken cancellationToken = default)
    {
        // Validate product exists and is active
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
        {
            throw new InvalidOperationException("PRODUCT_NOT_FOUND");
        }

        if (!product.IsActive)
        {
            throw new InvalidOperationException("PRODUCT_NOT_AVAILABLE");
        }

        if (request.Quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than 0", nameof(request.Quantity));
        }

        // Get or create cart
        var cart = await _cartRepository.GetActiveCartByUserIdAsync(userId, cancellationToken);
        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId,
                Status = "ACTIVE",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            cart = await _cartRepository.CreateCartAsync(cart, cancellationToken);
        }

        // Check if item already exists in cart
        var existingItem = await _cartRepository.GetCartItemByCartAndProductAsync(cart.Id, request.ProductId, cancellationToken);
        
        if (existingItem != null)
        {
            // Update quantity
            existingItem.Quantity += request.Quantity;
            existingItem.UnitPrice = product.Price; // Update price in case it changed
            await _cartRepository.UpdateCartItemAsync(existingItem, cancellationToken);
        }
        else
        {
            // Add new item
            var newItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = product.Id,
                Quantity = request.Quantity,
                UnitPrice = product.Price
            };
            await _cartRepository.AddCartItemAsync(newItem, cancellationToken);
        }

        // Update cart timestamp
        cart.UpdatedAt = DateTime.UtcNow;
        await _cartRepository.UpdateCartAsync(cart, cancellationToken);

        // Return updated cart
        return await GetCartAsync(userId, cancellationToken);
    }

    public async Task<CartDto> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemRequest request, CancellationToken cancellationToken = default)
    {
        var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId, cancellationToken);
        if (cartItem == null)
        {
            throw new InvalidOperationException("CART_ITEM_NOT_FOUND");
        }

        // Verify cart belongs to user
        var cart = await _cartRepository.GetActiveCartByUserIdAsync(userId, cancellationToken);
        if (cart == null || cart.Id != cartItem.CartId)
        {
            throw new UnauthorizedAccessException("CART_ACCESS_DENIED");
        }

        if (request.Quantity <= 0)
        {
            // Remove item if quantity is 0 or less
            await _cartRepository.DeleteCartItemAsync(cartItem, cancellationToken);
        }
        else
        {
            // Update product price in case it changed
            var product = await _productRepository.GetByIdAsync(cartItem.ProductId, cancellationToken);
            if (product != null)
            {
                cartItem.UnitPrice = product.Price;
            }

            cartItem.Quantity = request.Quantity;
            await _cartRepository.UpdateCartItemAsync(cartItem, cancellationToken);
        }

        // Update cart timestamp
        cart.UpdatedAt = DateTime.UtcNow;
        await _cartRepository.UpdateCartAsync(cart, cancellationToken);

        return await GetCartAsync(userId, cancellationToken);
    }

    public async Task<CartDto> RemoveCartItemAsync(int userId, int cartItemId, CancellationToken cancellationToken = default)
    {
        var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId, cancellationToken);
        if (cartItem == null)
        {
            throw new InvalidOperationException("CART_ITEM_NOT_FOUND");
        }

        // Verify cart belongs to user
        var cart = await _cartRepository.GetActiveCartByUserIdAsync(userId, cancellationToken);
        if (cart == null || cart.Id != cartItem.CartId)
        {
            throw new UnauthorizedAccessException("CART_ACCESS_DENIED");
        }

        await _cartRepository.DeleteCartItemAsync(cartItem, cancellationToken);

        // Update cart timestamp
        cart.UpdatedAt = DateTime.UtcNow;
        await _cartRepository.UpdateCartAsync(cart, cancellationToken);

        return await GetCartAsync(userId, cancellationToken);
    }

    public async Task ClearCartAsync(int userId, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetActiveCartByUserIdAsync(userId, cancellationToken);
        if (cart == null)
        {
            return; // Nothing to clear
        }

        await _cartRepository.ClearCartItemsAsync(cart.Id, cancellationToken);

        // Update cart timestamp
        cart.UpdatedAt = DateTime.UtcNow;
        await _cartRepository.UpdateCartAsync(cart, cancellationToken);
    }

    private CartDto MapToDto(Cart cart)
    {
        var items = cart.Items.Select(i => new CartItemDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductName = i.Product.Name,
            ProductCode = i.Product.Code,
            ProductImageUrl = i.Product.ImageUrl,
            CategoryName = i.Product.Category?.Name ?? "N/A",
            UnitPrice = i.UnitPrice,
            Quantity = i.Quantity,
            LineTotal = i.UnitPrice * i.Quantity
        }).ToList();

        return new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Status = cart.Status,
            Items = items,
            TotalAmount = items.Sum(i => i.LineTotal),
            CreatedAt = cart.CreatedAt,
            UpdatedAt = cart.UpdatedAt
        };
    }
}
