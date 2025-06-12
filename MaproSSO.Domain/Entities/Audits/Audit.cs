using MaproSSO.Domain.Common;
using MaproSSO.Domain.Entities.SSO;

namespace MaproSSO.Domain.Entities.Audits;

public class Audit : BaseAuditableEntity
{
    public Guid AuditId { get; set; }
    public Guid TenantId { get; set; }
    public Guid ProgramId { get; set; }
    public Guid AreaId { get; set; }
    public string AuditCode { get; set; } = string.Empty;
    public string AuditType { get; set; } = string.Empty; // Internal, External, Certification
    public Guid AuditorUserId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? ExecutedDate { get; set; }
    public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed, Cancelled
    public decimal? TotalScore { get; set; }
    public decimal? MaxScore { get; set; }
    public decimal? CompliancePercentage => MaxScore > 0 ? (TotalScore / MaxScore) * 100 : 0;

    // Navigation properties
    public virtual AuditProgram Program { get; set; } = null!;
    public virtual Areas.Area Area { get; set; } = null!;
    public virtual Security.User Auditor { get; set; } = null!;
    public virtual ICollection<AuditEvaluation> Evaluations { get; set; } = new List<AuditEvaluation>();
}