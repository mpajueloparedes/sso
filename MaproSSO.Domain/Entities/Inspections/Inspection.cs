using MaproSSO.Domain.Common;
using MaproSSO.Domain.Entities.SSO;
using MaproSSO.Domain.Enums;

namespace MaproSSO.Domain.Entities.Inspections;

public class Inspection : BaseAuditableEntity
{
    public Guid InspectionId { get; set; }
    public Guid TenantId { get; set; }
    public Guid ProgramId { get; set; }
    public Guid AreaId { get; set; }
    public string InspectionCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid InspectorUserId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? ExecutedDate { get; set; }
    public InspectionStatus Status { get; set; } = InspectionStatus.Pending; // Pending, InProgress, Completed
    public int CompletionPercentage { get; set; } = 0;
    public string? DocumentUrl { get; set; }

    // Navigation properties
    public virtual InspectionProgram Program { get; set; } = null!;
    public virtual Areas.Area Area { get; set; } = null!;
    public virtual Security.User Inspector { get; set; } = null!;
    public virtual ICollection<InspectionObservation> Observations { get; set; } = new List<InspectionObservation>();
}