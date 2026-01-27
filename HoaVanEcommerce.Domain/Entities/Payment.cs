using System;

namespace HoaVanEcommerce.Domain.Entities;

public class Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = null!; // e.g. BankTransfer, QRCode
    public string Status { get; set; } = null!; // Pending, Paid, Failed, Refunded
    public string? TransactionCode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }

    public Order Order { get; set; } = null!;
}

