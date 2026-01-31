namespace HoaVanEcommerce.BE.Application.DTOs.ShopSetting;

public class ShopSettingDto
{
    public int Id { get; set; }
    public string BankName { get; set; } = null!;
    public string AccountName { get; set; } = null!;
    public string AccountNumber { get; set; } = null!;
    public string QrImageUrl { get; set; } = null!;
    public string? Description { get; set; }
}
