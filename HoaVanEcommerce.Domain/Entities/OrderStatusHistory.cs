using System;

namespace HoaVanEcommerce.Domain.Entities;

public class OrderStatusHistory
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int? ChangedByUserId { get; set; }
    public string? FromStatus { get; set; }
    public string ToStatus { get; set; } = null!;
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Order Order { get; set; } = null!;
    public User? ChangedByUser { get; set; }
}

