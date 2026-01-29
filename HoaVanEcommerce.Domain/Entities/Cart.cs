using System;
using System.Collections.Generic;

namespace HoaVanEcommerce.Domain.Entities;

public class Cart
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}

