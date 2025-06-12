using MaproSSO.Domain.Common;
using MaproSSO.Domain.Entities.SSO;

namespace MaproSSO.Domain.Entities.Audits;

public class AuditCriteria : BaseEntity
{
    public Guid CriteriaId { get; set; }
    public Guid CategoryId { get; set; }
    public string CriteriaCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal MaxScore { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual AuditCategory Category { get; set; } = null!;
    public virtual ICollection<AuditEvaluation> Evaluations { get; set; } = new List<AuditEvaluation>();
}