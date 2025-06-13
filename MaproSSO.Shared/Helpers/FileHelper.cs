using MaproSSO.Shared.Constants;

namespace MaproSSO.Shared.Helpers;

public static class FileHelper
{
    public static string FormatFileSize(long bytes)
    {
        if (bytes == 0) return "0 B";

        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }

    public static string GetFileExtension(string fileName)
    {
        return Path.GetExtension(fileName).ToLowerInvariant();
    }

    public static bool IsImageFile(string fileName)
    {
        var extension = GetFileExtension(fileName);
        return SystemConstants.FileTypes.ALLOWED_IMAGE_EXTENSIONS.Contains(extension);
    }

    public static bool IsDocumentFile(string fileName)
    {
        var extension = GetFileExtension(fileName);
        return SystemConstants.FileTypes.ALLOWED_DOCUMENT_EXTENSIONS.Contains(extension);
    }

    public static bool IsVideoFile(string fileName)
    {
        var extension = GetFileExtension(fileName);
        return SystemConstants.FileTypes.ALLOWED_VIDEO_EXTENSIONS.Contains(extension);
    }

    public static string GetContentType(string fileName)
    {
        var extension = GetFileExtension(fileName);

        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".txt" => "text/plain",
            ".rtf" => "application/rtf",
            ".mp4" => "video/mp4",
            ".avi" => "video/avi",
            ".mov" => "video/quicktime",
            ".wmv" => "video/x-ms-wmv",
            ".flv" => "video/x-flv",
            ".webm" => "video/webm",
            _ => "application/octet-stream"
        };
    }

    public static bool IsFileSizeValid(long fileSize, string fileName)
    {
        if (IsImageFile(fileName))
        {
            return fileSize <= SystemConstants.FileTypes.MAX_IMAGE_SIZE;
        }

        if (IsVideoFile(fileName))
        {
            return fileSize <= SystemConstants.FileTypes.MAX_VIDEO_SIZE;
        }

        return fileSize <= SystemConstants.FileTypes.MAX_FILE_SIZE;
    }

    public static string GenerateUniqueFileName(string originalFileName)
    {
        var extension = GetFileExtension(originalFileName);
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
        var uniqueId = Guid.NewGuid().ToString("N")[..8];

        return $"{nameWithoutExtension}_{uniqueId}{extension}";
    }
}