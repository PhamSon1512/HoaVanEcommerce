namespace HoaVanEcommerce.BE.Application.DTOs.Orders;

public class OrderDto
{
    public int Id { get; set; }
    public string OrderCode { get; set; } = null!;
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public string PaymentStatus { get; set; } = null!;
    public string ShippingStatus { get; set; } = null!;
    public string OrderStatus { get; set; } = null!;
    public string PaymentMethod { get; set; } = null!;
    public string? CurrentStatusNote { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public OrderShippingInfoDto? ShippingInfo { get; set; }
}

public class OrderItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public string ProductImageUrl { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public class OrderShippingInfoDto
{
    public string ReceiverName { get; set; } = null!;
    public string ReceiverPhone { get; set; } = null!;
    public string FullAddress { get; set; } = null!;
    public string? Note { get; set; }
}

public class CreateOrderRequest
{
    public List<int> CartItemIds { get; set; } = new(); // IDs của các cart items muốn thanh toán
    public string? ShippingName { get; set; }
    public string? ShippingPhone { get; set; }
    public string? ShippingAddress { get; set; }
    public string? ShippingNote { get; set; }
}

public class UpdateOrderStatusRequest
{
    public string OrderStatus { get; set; } = null!;
    public string? PaymentStatus { get; set; }
    public string? ShippingStatus { get; set; }
    public string? Note { get; set; }
}

public class ConfirmPaymentRequest
{
    public string? TransactionRef { get; set; }
}

public class ConfirmReceivedRequest
{
    public string? Note { get; set; }
}
