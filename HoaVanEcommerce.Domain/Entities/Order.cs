using System;
using System.Collections.Generic;

namespace HoaVanEcommerce.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string OrderCode { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = null!; // matching order_status enum/string in DB
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    public OrderShippingInfo? ShippingInfo { get; set; }
}

