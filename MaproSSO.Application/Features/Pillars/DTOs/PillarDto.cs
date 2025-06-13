namespace MaproSSO.Application.Features.Pillars.DTOs;

public class PillarDto
{
    public Guid PillarId { get; set; }
    public Guid TenantId { get; set; }
    public string PillarName { get; set; } = string.Empty;
    public string PillarCode { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TotalFolders { get; set; }
    public int TotalDocuments { get; set; }
    public List<DocumentFolderDto> RootFolders { get; set; } = new();
}

public class DocumentFolderDto
{
    public Guid FolderId { get; set; }
    public Guid TenantId { get; set; }
    public Guid PillarId { get; set; }
    public string PillarName { get; set; } = string.Empty;
    public Guid AreaId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public string FolderName { get; set; } = string.Empty;
    public Guid? ParentFolderId { get; set; }
    public string? ParentFolderName { get; set; }
    public string Path { get; set; } = string.Empty;
    public bool IsSystemFolder { get; set; }
    public DateTime CreatedAt { get; set; }
    public int DocumentCount { get; set; }
    public int SubFolderCount { get; set; }
    public List<DocumentFolderDto> SubFolders { get; set; } = new();
    public List<DocumentDto> Documents { get; set; } = new();
}

public class DocumentDto
{
    public Guid DocumentId { get; set; }
    public Guid TenantId { get; set; }
    public Guid FolderId { get; set; }
    public string FolderName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string FileSizeFormatted { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string StorageUrl { get; set; } = string.Empty;
    public int Version { get; set; }
    public bool IsCurrentVersion { get; set; }
    public Guid? ParentDocumentId { get; set; }
    public string? Tags { get; set; }
    public List<string> TagList { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedByName { get; set; }
    public bool IsDeleted { get; set; }
    public List<DocumentVersionDto> Versions { get; set; } = new();
}

public class DocumentVersionDto
{
    public Guid DocumentId { get; set; }
    public int Version { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string FileSizeFormatted { get; set; } = string.Empty;
    public string StorageUrl { get; set; } = string.Empty;
    public bool IsCurrentVersion { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
}

public class FolderTreeDto
{
    public Guid FolderId { get; set; }
    public string FolderName { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public Guid? ParentFolderId { get; set; }
    public bool IsSystemFolder { get; set; }
    public int Level { get; set; }
    public int DocumentCount { get; set; }
    public List<FolderTreeDto> Children { get; set; } = new();
}