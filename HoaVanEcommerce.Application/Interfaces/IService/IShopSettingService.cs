using HoaVanEcommerce.BE.Application.DTOs.ShopSetting;

namespace HoaVanEcommerce.BE.Application.Interfaces;

public interface IShopSettingService
{
    Task<ShopSettingDto> GetShopSettingAsync(CancellationToken cancellationToken = default);
    Task<ShopSettingDto> UpdateShopSettingAsync(UpdateShopSettingRequest request, CancellationToken cancellationToken = default);
    Task<ShopSettingDto> UpdateShopSettingWithQrAsync(UpdateShopSettingWithQrRequest request, string qrImageUrl, CancellationToken cancellationToken = default);
}
