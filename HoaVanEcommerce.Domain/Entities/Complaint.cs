using System;

namespace HoaVanEcommerce.Domain.Entities;

public class Complaint
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string Status { get; set; } = null!; // Pending, InReview, Resolved, Rejected
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }

    public Order Order { get; set; } = null!;
    public User User { get; set; } = null!;
}

