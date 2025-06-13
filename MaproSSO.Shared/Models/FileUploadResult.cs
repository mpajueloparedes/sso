using System.Text.Json.Serialization;

namespace MaproSSO.Shared.Models;

public class FileUploadResult
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("fileName")]
    public string FileName { get; set; } = string.Empty;

    [JsonPropertyName("originalFileName")]
    public string OriginalFileName { get; set; } = string.Empty;

    [JsonPropertyName("fileUrl")]
    public string FileUrl { get; set; } = string.Empty;

    [JsonPropertyName("fileSize")]
    public long FileSize { get; set; }

    [JsonPropertyName("contentType")]
    public string ContentType { get; set; } = string.Empty;

    [JsonPropertyName("uploadedAt")]
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("error")]
    public string? Error { get; set; }
}