using MaproSSO.Domain.Common;
using MaproSSO.Domain.Entities.SSO;

namespace MaproSSO.Domain.Entities.Inspections;

public class InspectionProgram : BaseAggregateRoot
{
    public Guid ProgramId { get; set; }
    public Guid TenantId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public string ProgramType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Year { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<InspectionProgramDetail> ProgramDetails { get; set; } = new List<InspectionProgramDetail>();
    public virtual ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();
}
