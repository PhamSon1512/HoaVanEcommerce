using System;
using System.Collections.Generic;

namespace HoaVanEcommerce.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = null!;
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Category Category { get; set; } = null!;
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

