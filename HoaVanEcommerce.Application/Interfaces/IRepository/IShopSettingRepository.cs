using HoaVanEcommerce.Domain.Entities;

namespace HoaVanEcommerce.BE.Application.Interfaces;

public interface IShopSettingRepository
{
    Task<ShopSetting?> GetFirstAsync(CancellationToken cancellationToken = default);
    Task<ShopSetting> CreateOrUpdateAsync(ShopSetting setting, CancellationToken cancellationToken = default);
}
