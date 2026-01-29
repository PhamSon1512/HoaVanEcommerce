namespace HoaVanEcommerce.Domain.Entities;

public class OrderShippingInfo
{
    public int OrderId { get; set; }
    public string ReceiverName { get; set; } = null!;
    public string ReceiverPhone { get; set; } = null!;
    public string FullAddress { get; set; } = null!;
    public string? Note { get; set; }

    public Order Order { get; set; } = null!;
}

