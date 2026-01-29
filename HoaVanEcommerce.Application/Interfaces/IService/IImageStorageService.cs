namespace HoaVanEcommerce.BE.Application.Interfaces;

public interface IImageStorageService
{
    /// <summary>
    /// Upload image content and return a publicly accessible URL.
    /// The Web layer should provide the file stream and metadata (name/content-type).
    /// </summary>
    Task<string> UploadProductImageAsync(
        Stream content,
        string fileName,
        string? contentType,
        CancellationToken cancellationToken = default);
}

