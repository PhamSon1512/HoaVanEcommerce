using System;
using System.Collections.Generic;

namespace HoaVanEcommerce.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public string OrderCode { get; set; } = null!;
    public int UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentStatus { get; set; } = "PENDING";
    public string ShippingStatus { get; set; } = "NOT_SHIPPED";
    public string OrderStatus { get; set; } = "PENDING_PAYMENT";
    public string PaymentMethod { get; set; } = "BANK_TRANSFER_QR";
    public string? CurrentStatusNote { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    public OrderShippingInfo? ShippingInfo { get; set; }
}

