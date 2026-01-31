using HoaVanEcommerce.BE.Application.DTOs.ShopSetting;
using HoaVanEcommerce.BE.Application.Interfaces;
using HoaVanEcommerce.BE.Models;
using Microsoft.AspNetCore.Mvc;

namespace HoaVanEcommerce.BE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShopSettingController : ControllerBase
{
    private readonly IShopSettingService _shopSettingService;
    private readonly IImageStorageService _imageStorage;

    public ShopSettingController(IShopSettingService shopSettingService, IImageStorageService imageStorage)
    {
        _shopSettingService = shopSettingService;
        _imageStorage = imageStorage;
    }

    [HttpGet]
    public async Task<ActionResult<ShopSettingDto>> GetShopSetting(CancellationToken cancellationToken)
    {
        try
        {
            var setting = await _shopSettingService.GetShopSettingAsync(cancellationToken);
            return Ok(setting);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut]
    [Consumes("multipart/form-data")]
    // TODO: Add [Authorize] and admin role check
    public async Task<ActionResult<ShopSettingDto>> UpdateShopSetting([FromForm] UpdateShopSettingForm form, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            string qrImageUrl;

            if (form.QrImage != null)
            {
                // Upload QR image to Cloudinary (using product image upload method)
                using var stream = form.QrImage.OpenReadStream();
                qrImageUrl = await _imageStorage.UploadProductImageAsync(
                    stream,
                    form.QrImage.FileName,
                    form.QrImage.ContentType,
                    cancellationToken
                );
            }
            else
            {
                // Get existing QR URL if not uploading new image
                var existing = await _shopSettingService.GetShopSettingAsync(cancellationToken);
                qrImageUrl = existing.QrImageUrl;
            }

            var request = new UpdateShopSettingWithQrRequest
            {
                BankName = form.BankName,
                AccountName = form.AccountName,
                AccountNumber = form.AccountNumber,
                Description = form.Description
            };

            var setting = await _shopSettingService.UpdateShopSettingWithQrAsync(request, qrImageUrl, cancellationToken);
            return Ok(setting);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Có lỗi xảy ra khi cập nhật cài đặt cửa hàng.", error = ex.Message });
        }
    }
}
