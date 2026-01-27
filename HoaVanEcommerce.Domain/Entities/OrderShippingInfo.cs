using System;

namespace HoaVanEcommerce.Domain.Entities;

public class OrderShippingInfo
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string ReceiverName { get; set; } = null!;
    public string ReceiverPhone { get; set; } = null!;
    public string AddressLine { get; set; } = null!;
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? Province { get; set; }
    public decimal ShippingFee { get; set; }
    public string? ShipperName { get; set; }
    public string? TrackingCode { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }

    public Order Order { get; set; } = null!;
}

