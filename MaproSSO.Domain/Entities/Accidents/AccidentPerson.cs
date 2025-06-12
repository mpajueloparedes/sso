using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.Accidents;

public class AccidentPerson : BaseEntity
{
    public Guid PersonId { get; set; }
    public Guid AccidentId { get; set; }
    public string PersonType { get; set; } = string.Empty; // Affected, Responsible, Witness
    public Guid? UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? DocumentNumber { get; set; }
    public int? Age { get; set; }
    public string? Gender { get; set; }
    public string? Position { get; set; }
    public string? Company { get; set; }
    public Guid? AreaId { get; set; }
    public string? InjuryType { get; set; }
    public string? InjuryDescription { get; set; }
    public bool MedicalAttention { get; set; } = false;
    public int? LostWorkDays { get; set; }

    // Navigation properties
    public virtual Accident Accident { get; set; } = null!;
    public virtual Security.User? User { get; set; }
    public virtual Areas.Area? Area { get; set; }
}