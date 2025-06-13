using MaproSSO.Application.Common.Interfaces;

namespace MaproSSO.Infrastructure.Services;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<Stream> DownloadFileAsync(string fileUrl, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);
    Task<string> GetFileUrlAsync(string fileKey, TimeSpan? expiration = null);
    Task<bool> FileExistsAsync(string fileUrl, CancellationToken cancellationToken = default);
}

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(IConfiguration configuration, ILogger<LocalFileStorageService> logger)
    {
        _basePath = configuration.GetValue<string>("FileStorage:LocalPath") ?? "wwwroot/uploads";
        _logger = logger;

        // Ensure directory exists
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        try
        {
            // Generate unique file name to avoid conflicts
            var fileExtension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

            // Create year/month directory structure
            var year = DateTime.UtcNow.Year.ToString();
            var month = DateTime.UtcNow.Month.ToString("00");
            var directoryPath = Path.Combine(_basePath, year, month);

            Directory.CreateDirectory(directoryPath);

            var filePath = Path.Combine(directoryPath, uniqueFileName);

            using (var fileStreamDestination = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileStreamDestination, cancellationToken);
            }

            // Return relative URL
            return $"/uploads/{year}/{month}/{uniqueFileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName}", fileName);
            throw;
        }
    }

    public async Task<Stream> DownloadFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var filePath = GetPhysicalPath(fileUrl);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {fileUrl}");
            }

            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file {FileUrl}", fileUrl);
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var filePath = GetPhysicalPath(fileUrl);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FileUrl}", fileUrl);
            return false;
        }
    }

    public async Task<string> GetFileUrlAsync(string fileKey, TimeSpan? expiration = null)
    {
        // For local storage, return the direct URL
        return fileKey;
    }

    public async Task<bool> FileExistsAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var filePath = GetPhysicalPath(fileUrl);
            return File.Exists(filePath);
        }
        catch
        {
            return false;
        }
    }

    private string GetPhysicalPath(string fileUrl)
    {
        // Remove leading slash and convert to physical path
        var relativePath = fileUrl.TrimStart('/').Replace("/uploads/", "");
        return Path.Combine(_basePath, relativePath);
    }
}
