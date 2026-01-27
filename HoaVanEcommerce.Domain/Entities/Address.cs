using System;

namespace HoaVanEcommerce.Domain.Entities;

public class Address
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string AddressLine { get; set; } = null!;
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? Province { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}

