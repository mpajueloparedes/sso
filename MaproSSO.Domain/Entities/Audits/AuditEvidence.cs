//using MaproSSO.Domain.Common;

//namespace MaproSSO.Domain.Entities.Audits;

//public class AuditEvidence : BaseAuditableEntity
//{
//    public Guid EvidenceId { get; set; }
//    public Guid EvaluationId { get; set; }
//    public string Description { get; set; } = string.Empty;
//    public string EvidenceUrl { get; set; } = string.Empty;
//    public Guid UploadedBy { get; set; }

//    // Navigation properties
//    public virtual AuditEvaluation Evaluation { get; set; } = null!;
//    public