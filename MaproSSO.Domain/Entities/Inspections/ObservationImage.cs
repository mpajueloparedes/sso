using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.Inspections;

public class ObservationImage : BaseEntity
{
    public Guid ImageId { get; set; }
    public Guid ObservationId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; } = 0;

    // Navigation properties
    public virtual InspectionObservation Observation { get; set; } = null!;
}