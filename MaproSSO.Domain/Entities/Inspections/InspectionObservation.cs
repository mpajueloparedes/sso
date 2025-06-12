using MaproSSO.Domain.Common;
using MaproSSO.Domain.Entities.SSO;

namespace MaproSSO.Domain.Entities.Inspections;

public class InspectionObservation : BaseAuditableEntity
{
    public Guid ObservationId { get; set; }
    public Guid InspectionId { get; set; }
    public string ObservationCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Safety, Quality, Environment, Compliance
    public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
    public Guid ResponsibleUserId { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed
    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    public virtual Inspection Inspection { get; set; } = null!;
    public virtual Security.User ResponsibleUser { get; set; } = null!;
    public virtual ICollection<ObservationImage> Images { get; set; } = new List<ObservationImage>();
    public virtual ICollection<ObservationEvidence> Evidences { get; set; } = new List<ObservationEvidence>();
}