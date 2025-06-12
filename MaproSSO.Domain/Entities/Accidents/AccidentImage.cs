using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.Accidents;

public class AccidentImage : BaseEntity
{
    public Guid ImageId { get; set; }
    public Guid AccidentId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; } = 0;

    // Navigation properties
    public virtual Accident Accident { get; set; } = null!;
}