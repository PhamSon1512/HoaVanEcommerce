using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HoaVanEcommerce.BE.Application.Interfaces;
using HoaVanEcommerce.BE.Application.Settings;
using Microsoft.Extensions.Options;

namespace HoaVanEcommerce.Infrastructure.Services;

public sealed class CloudinaryImageStorageService : IImageStorageService
{
    private readonly CloudinarySettings _settings;
    private readonly Cloudinary _cloudinary;

    public CloudinaryImageStorageService(IOptions<CloudinarySettings> options)
    {
        _settings = options.Value;
        if (string.IsNullOrWhiteSpace(_settings.CloudName) ||
            string.IsNullOrWhiteSpace(_settings.ApiKey) ||
            string.IsNullOrWhiteSpace(_settings.ApiSecret))
        {
            throw new InvalidOperationException("CLOUDINARY_NOT_CONFIGURED");
        }
        var account = new Account(_settings.CloudName, _settings.ApiKey, _settings.ApiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadProductImageAsync(
        Stream content,
        string fileName,
        string? contentType,
        CancellationToken cancellationToken = default)
    {
        if (content == null)
        {
            throw new InvalidOperationException("FILE_EMPTY");
        }

        // Basic content-type allow list
        var ct = contentType?.ToLowerInvariant();
        if (ct is not ("image/jpeg" or "image/png" or "image/webp"))
        {
            throw new InvalidOperationException("INVALID_FILE_TYPE");
        }

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, content),
            Folder = _settings.ProductFolder,
            UseFilename = true,
            UniqueFilename = true,
            Overwrite = false
        };

        var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

        if (result.StatusCode is not System.Net.HttpStatusCode.OK and not System.Net.HttpStatusCode.Created)
        {
            throw new InvalidOperationException("UPLOAD_FAILED");
        }

        var url = result.SecureUrl?.ToString() ?? result.Url?.ToString();
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new InvalidOperationException("UPLOAD_NO_URL");
        }

        return url;
    }
}

