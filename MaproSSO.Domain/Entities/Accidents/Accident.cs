using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.Accidents;

public class Accident : BaseAuditableEntity
{
    public Guid AccidentId { get; set; }
    public Guid TenantId { get; set; }
    public string AccidentCode { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Accident, Incident, NearMiss
    public string Severity { get; set; } = string.Empty; // Minor, Moderate, Serious, Fatal
    public DateTime OccurredAt { get; set; }
    public DateTime ReportedAt { get; set; }
    public string Shift { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImmediateCauses { get; set; }
    public string? RootCauses { get; set; }
    public string? WitnessNames { get; set; }
    public string Status { get; set; } = "Reported"; // Reported, UnderInvestigation, Closed
    public DateTime? InvestigationStartDate { get; set; }
    public DateTime? InvestigationEndDate { get; set; }

    // Navigation properties
    public virtual ICollection<AccidentPerson> People { get; set; } = new List<AccidentPerson>();
    public virtual ICollection<AccidentImage> Images { get; set; } = new List<AccidentImage>();
}