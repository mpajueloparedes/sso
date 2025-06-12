using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.Trainings;

public class Training : BaseAuditableEntity
{
    public Guid TrainingId { get; set; }
    public Guid TenantId { get; set; }
    public string TrainingCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TrainingType { get; set; } = string.Empty; // Induction, Periodic, Specific, Emergency
    public string Mode { get; set; } = string.Empty; // Presential, Virtual, Mixed
    public Guid? InstructorUserId { get; set; }
    public string? ExternalInstructor { get; set; }
    public Guid AreaId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public int Duration { get; set; } // En minutos
    public string? Location { get; set; }
    public int? MaxParticipants { get; set; }
    public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed, Cancelled
    public string? MaterialUrl { get; set; }

    // Navigation properties
    public virtual Security.User? Instructor { get; set; }
    public virtual Areas.Area Area { get; set; } = null!;
    public virtual ICollection<TrainingParticipant> Participants { get; set; } = new List<TrainingParticipant>();
}