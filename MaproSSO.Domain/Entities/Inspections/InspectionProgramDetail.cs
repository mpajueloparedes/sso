using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.Inspections;

public class InspectionProgramDetail : BaseEntity
{
    public Guid DetailId { get; set; }
    public Guid ProgramId { get; set; }
    public Guid AreaId { get; set; }
    public string Frequency { get; set; } = string.Empty; // Daily, Weekly, Monthly, Quarterly, Annual
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual InspectionProgram Program { get; set; } = null!;
    public virtual Areas.Area Area { get; set; } = null!;
}