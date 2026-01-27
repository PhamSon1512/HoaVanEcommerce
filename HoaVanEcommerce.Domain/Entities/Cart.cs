using System;
using System.Collections.Generic;

namespace HoaVanEcommerce.Domain.Entities;

public class Cart
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}

