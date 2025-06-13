using MaproSSO.Domain.Common;
using MaproSSO.Domain.Entities.SSO;

namespace MaproSSO.Domain.Entities.Audits;

public class AuditProgram : BaseAggregateRoot
{
    public Guid ProgramId { get; set; }
    public Guid TenantId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Standard { get; set; } = string.Empty; // ISO 45001, OHSAS 18001, etc.
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<Audit> Audits { get; set; } = new List<Audit>();
}