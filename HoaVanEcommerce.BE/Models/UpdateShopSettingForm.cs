namespace HoaVanEcommerce.BE.Models;

public class UpdateShopSettingForm
{
    public string BankName { get; set; } = null!;
    public string AccountName { get; set; } = null!;
    public string AccountNumber { get; set; } = null!;
    public string? Description { get; set; }
    public IFormFile? QrImage { get; set; }
}
