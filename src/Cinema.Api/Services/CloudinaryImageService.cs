using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace Cinema.Api.Services;

public class CloudinaryImageService
{
    private readonly Cloudinary _cloudinary;
    private readonly ILogger<CloudinaryImageService> _logger;

    public CloudinaryImageService(IOptions<CloudinarySettings> settings, ILogger<CloudinaryImageService> logger)
    {
        _logger = logger;

        var account = new Account(
            settings.Value.CloudName,
            settings.Value.ApiKey,
            settings.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(account);
    }

    /// <summary>
    /// Uploads an image to Cloudinary from a base64 string or file stream
    /// </summary>
    public async Task<string> UploadImageAsync(Stream imageStream, string fileName, string folder = "movies")
    {
        try
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, imageStream),
                Folder = folder,
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                _logger.LogError($"Cloudinary upload error: {uploadResult.Error.Message}");
                throw new Exception($"Image upload failed: {uploadResult.Error.Message}");
            }

            _logger.LogInformation($"Image uploaded successfully: {uploadResult.SecureUrl}");
            return uploadResult.SecureUrl.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image to Cloudinary");
            throw;
        }
    }

    /// <summary>
    /// Uploads an image from base64 string
    /// </summary>
    public async Task<string> UploadImageFromBase64Async(string base64Image, string fileName, string folder = "movies")
    {
        try
        {
            // Remove data:image/xxx;base64, prefix if present
            var base64Data = base64Image.Contains(",")
                ? base64Image.Split(',')[1]
                : base64Image;

            var imageBytes = Convert.FromBase64String(base64Data);

            using var stream = new MemoryStream(imageBytes);
            return await UploadImageAsync(stream, fileName, folder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading base64 image to Cloudinary");
            throw;
        }
    }

    /// <summary>
    /// Deletes an image from Cloudinary by public ID
    /// </summary>
    public async Task<bool> DeleteImageAsync(string publicId)
    {
        try
        {
            var deletionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParams);

            if (result.Result == "ok")
            {
                _logger.LogInformation($"Image deleted successfully: {publicId}");
                return true;
            }

            _logger.LogWarning($"Image deletion failed: {result.Result}");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image from Cloudinary");
            return false;
        }
    }

    /// <summary>
    /// Extracts public ID from Cloudinary URL
    /// </summary>
    public string? GetPublicIdFromUrl(string imageUrl)
    {
        try
        {
            // Example URL: https://res.cloudinary.com/dntcviwyy/image/upload/v1732586169/movies/dune_2_fz0cul.jpg
            // Public ID: movies/dune_2_fz0cul

            var uri = new Uri(imageUrl);
            var segments = uri.AbsolutePath.Split('/');

            // Find the upload segment
            var uploadIndex = Array.FindIndex(segments, s => s == "upload");
            if (uploadIndex == -1 || uploadIndex >= segments.Length - 2)
                return null;

            // Skip the version (v1732586169) and get the rest
            var publicIdParts = segments.Skip(uploadIndex + 2).ToArray();
            var publicId = string.Join("/", publicIdParts);

            // Remove file extension
            publicId = System.IO.Path.ChangeExtension(publicId, null)?.TrimEnd('.');

            return publicId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting public ID from URL");
            return null;
        }
    }
}

public class CloudinarySettings
{
    public string CloudName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
}
