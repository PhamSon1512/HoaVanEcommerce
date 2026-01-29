using System;

namespace HoaVanEcommerce.Domain.Entities;

public class ShopSetting
{
    public int Id { get; set; }
    public string BankName { get; set; } = null!;
    public string AccountName { get; set; } = null!;
    public string AccountNumber { get; set; } = null!;
    public string QrImageUrl { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

