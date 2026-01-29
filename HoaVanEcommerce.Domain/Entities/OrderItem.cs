namespace HoaVanEcommerce.Domain.Entities;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }

    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

