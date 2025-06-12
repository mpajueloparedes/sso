using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.Trainings;

public class TrainingParticipant : BaseAuditableEntity
{
    public Guid ParticipantId { get; set; }
    public Guid TrainingId { get; set; }
    public Guid UserId { get; set; }
    public string AttendanceStatus { get; set; } = "Registered"; // Registered, Present, Absent, Excused
    public decimal? Score { get; set; }
    public bool? Passed { get; set; }
    public string? CertificateUrl { get; set; }
    public string? Comments { get; set; }
    public DateTime RegisteredAt { get; set; }
    public DateTime? AttendanceMarkedAt { get; set; }

    // Navigation properties
    public virtual Training Training { get; set; } = null!;
    public virtual Security.User User { get; set; } = null!;
}
