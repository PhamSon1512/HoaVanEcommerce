namespace HoaVanEcommerce.BE.Application.DTOs.Cart;

public class CartDto
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string Status { get; set; } = null!;
    public List<CartItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CartItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string? ProductCode { get; set; }
    public string ProductImageUrl { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}

public class AddToCartRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class UpdateCartItemRequest
{
    public int Quantity { get; set; }
}
