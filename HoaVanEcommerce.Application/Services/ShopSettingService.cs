using HoaVanEcommerce.BE.Application.DTOs.ShopSetting;
using HoaVanEcommerce.BE.Application.Interfaces;

namespace HoaVanEcommerce.BE.Application.Services;

public class ShopSettingService : IShopSettingService
{
    private readonly IShopSettingRepository _shopSettingRepository;

    public ShopSettingService(IShopSettingRepository shopSettingRepository)
    {
        _shopSettingRepository = shopSettingRepository;
    }

    public async Task<ShopSettingDto> GetShopSettingAsync(CancellationToken cancellationToken = default)
    {
        var setting = await _shopSettingRepository.GetFirstAsync(cancellationToken);
        if (setting == null)
        {
            throw new InvalidOperationException("Shop setting not found. Please configure shop settings first.");
        }

        return new ShopSettingDto
        {
            Id = setting.Id,
            BankName = setting.BankName,
            AccountName = setting.AccountName,
            AccountNumber = setting.AccountNumber,
            QrImageUrl = setting.QrImageUrl,
            Description = setting.Description
        };
    }

    public async Task<ShopSettingDto> UpdateShopSettingAsync(UpdateShopSettingRequest request, CancellationToken cancellationToken = default)
    {
        var setting = await _shopSettingRepository.GetFirstAsync(cancellationToken);
        if (setting == null)
        {
            throw new InvalidOperationException("Shop setting not found. Please create shop settings first.");
        }

        setting.BankName = request.BankName;
        setting.AccountName = request.AccountName;
        setting.AccountNumber = request.AccountNumber;
        setting.Description = request.Description;
        setting.UpdatedAt = DateTime.UtcNow;

        setting = await _shopSettingRepository.CreateOrUpdateAsync(setting, cancellationToken);

        return new ShopSettingDto
        {
            Id = setting.Id,
            BankName = setting.BankName,
            AccountName = setting.AccountName,
            AccountNumber = setting.AccountNumber,
            QrImageUrl = setting.QrImageUrl,
            Description = setting.Description
        };
    }

    public async Task<ShopSettingDto> UpdateShopSettingWithQrAsync(UpdateShopSettingWithQrRequest request, string qrImageUrl, CancellationToken cancellationToken = default)
    {
        var setting = await _shopSettingRepository.GetFirstAsync(cancellationToken);
        if (setting == null)
        {
            // Create new if doesn't exist
            setting = new Domain.Entities.ShopSetting
            {
                BankName = request.BankName,
                AccountName = request.AccountName,
                AccountNumber = request.AccountNumber,
                QrImageUrl = qrImageUrl,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
        else
        {
            setting.BankName = request.BankName;
            setting.AccountName = request.AccountName;
            setting.AccountNumber = request.AccountNumber;
            setting.QrImageUrl = qrImageUrl;
            setting.Description = request.Description;
            setting.UpdatedAt = DateTime.UtcNow;
        }

        setting = await _shopSettingRepository.CreateOrUpdateAsync(setting, cancellationToken);

        return new ShopSettingDto
        {
            Id = setting.Id,
            BankName = setting.BankName,
            AccountName = setting.AccountName,
            AccountNumber = setting.AccountNumber,
            QrImageUrl = setting.QrImageUrl,
            Description = setting.Description
        };
    }
}
