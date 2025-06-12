using System;
using System.IO;
using System.Threading.Tasks;

namespace MaproSSO.Application.Common.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, string containerName);
        Task<Stream> DownloadAsync(string fileUrl);
        Task<bool> DeleteAsync(string fileUrl);
        Task<bool> ExistsAsync(string fileUrl);
        Task<string> GetUrlWithSasTokenAsync(string fileUrl, TimeSpan expiry);
        string GetFileUrl(string containerName, string fileName);
    }
}