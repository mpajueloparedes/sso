using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.Pillars;

public class DocumentFolder : BaseAggregateRoot
{
    public Guid FolderId { get; set; }
    public Guid TenantId { get; set; }
    public Guid PillarId { get; set; }
    public Guid AreaId { get; set; }
    public string FolderName { get; set; } = string.Empty;
    public Guid? ParentFolderId { get; set; }
    public string Path { get; set; } = string.Empty;
    public bool IsSystemFolder { get; set; } = false;

    // Navigation properties
    public virtual Pillar Pillar { get; set; } = null!;
    public virtual Areas.Area Area { get; set; } = null!;
    public virtual DocumentFolder? ParentFolder { get; set; }
    public virtual ICollection<DocumentFolder> SubFolders { get; set; } = new List<DocumentFolder>();
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
}