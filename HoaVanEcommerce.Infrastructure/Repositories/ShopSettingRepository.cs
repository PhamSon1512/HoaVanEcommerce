using HoaVanEcommerce.BE.Application.Interfaces;
using HoaVanEcommerce.Domain.Entities;
using HoaVanEcommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HoaVanEcommerce.Infrastructure.Repositories;

public class ShopSettingRepository : IShopSettingRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ShopSettingRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ShopSetting?> GetFirstAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.ShopSettings.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ShopSetting> CreateOrUpdateAsync(ShopSetting setting, CancellationToken cancellationToken = default)
    {
        var existing = await GetFirstAsync(cancellationToken);
        if (existing != null)
        {
            existing.BankName = setting.BankName;
            existing.AccountName = setting.AccountName;
            existing.AccountNumber = setting.AccountNumber;
            existing.QrImageUrl = setting.QrImageUrl;
            existing.Description = setting.Description;
            existing.UpdatedAt = DateTime.UtcNow;
            _dbContext.ShopSettings.Update(existing);
        }
        else
        {
            await _dbContext.ShopSettings.AddAsync(setting, cancellationToken);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
        return existing ?? setting;
    }
}
