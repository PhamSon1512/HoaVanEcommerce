namespace HoaVanEcommerce.BE.Application.DTOs.ShopSetting;

public class UpdateShopSettingRequest
{
    public string BankName { get; set; } = null!;
    public string AccountName { get; set; } = null!;
    public string AccountNumber { get; set; } = null!;
    public string? Description { get; set; }
}

public class UpdateShopSettingWithQrRequest
{
    public string BankName { get; set; } = null!;
    public string AccountName { get; set; } = null!;
    public string AccountNumber { get; set; } = null!;
    public string? Description { get; set; }
    // QR Image will be uploaded as IFormFile and stored in Cloudinary
}
