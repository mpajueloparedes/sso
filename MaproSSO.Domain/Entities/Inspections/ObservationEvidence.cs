using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.Inspections;

public class ObservationEvidence : BaseAuditableEntity
{
    public Guid EvidenceId { get; set; }
    public Guid ObservationId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? EvidenceUrl { get; set; }
    public Guid UploadedBy { get; set; }

    // Navigation properties
    public virtual InspectionObservation Observation { get; set; } = null!;
    public virtual Security.User UploadedByUser { get; set; } = null!;
}