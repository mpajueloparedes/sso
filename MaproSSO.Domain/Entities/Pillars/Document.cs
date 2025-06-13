using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.Pillars;

public class Document : BaseAggregateRoot
{
    public Guid DocumentId { get; set; }
    public Guid TenantId { get; set; }
    public Guid FolderId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string StorageUrl { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public bool IsCurrentVersion { get; set; } = true;
    public Guid? ParentDocumentId { get; set; }
    public string? Tags { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    // Navigation properties
    public virtual DocumentFolder Folder { get; set; } = null!;
    public virtual Document? ParentDocument { get; set; }
    public virtual ICollection<Document> DocumentVersions { get; set; } = new List<Document>();
    public virtual Security.User CreatedByUser { get; set; } = null!;
    public virtual Security.User? UpdatedByUser { get; set; }
    public virtual Security.User? DeletedByUser { get; set; }
}