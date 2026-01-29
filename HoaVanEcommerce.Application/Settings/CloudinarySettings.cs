namespace HoaVanEcommerce.BE.Application.Settings;

public sealed class CloudinarySettings
{
    public string CloudName { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public string ApiSecret { get; set; } = null!;
    public string ProductFolder { get; set; } = "hoavan/products";
}

