using System;

namespace HoaVanEcommerce.Domain.Entities;

public class Complaint
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public string Type { get; set; } = null!;
    public string? Description { get; set; }
    public string Status { get; set; } = "OPEN";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Order Order { get; set; } = null!;
    public User User { get; set; } = null!;
}

