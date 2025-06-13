using MaproSSO.Domain.Common;
using MaproSSO.Domain.Entities.SSO;

namespace MaproSSO.Domain.Entities.Pillars;

public class Pillar : BaseAggregateRoot
{
    public Guid PillarId { get; set; }
    public Guid TenantId { get; set; }
    public string PillarName { get; set; } = string.Empty;
    public string PillarCode { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<DocumentFolder> DocumentFolders { get; set; } = new List<DocumentFolder>();
}