using MaproSSO.Domain.Common;
using MaproSSO.Domain.Entities.SSO;

namespace MaproSSO.Domain.Entities.Audits;

public class AuditEvaluation : BaseAuditableEntity
{
    public Guid EvaluationId { get; set; }
    public Guid AuditId { get; set; }
    public Guid CriteriaId { get; set; }
    public decimal Score { get; set; }
    public string? Observations { get; set; }
    public bool EvidenceRequired { get; set; } = false;
    public DateTime EvaluatedAt { get; set; }
    public Guid EvaluatedBy { get; set; }

    // Navigation properties
    public virtual Audit Audit { get; set; } = null!;
    public virtual AuditCriteria Criteria { get; set; } = null!;
    public virtual Security.User EvaluatedByUser { get; set; } = null!;
    public virtual ICollection<AuditEvidence> Evidences { get; set; } = new List<AuditEvidence>();
}