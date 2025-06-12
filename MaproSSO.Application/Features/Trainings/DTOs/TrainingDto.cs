namespace MaproSSO.Application.Features.Trainings.DTOs;

public class TrainingDto
{
    public Guid TrainingId { get; set; }
    public Guid TenantId { get; set; }
    public string TrainingCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TrainingType { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public Guid? InstructorUserId { get; set; }
    public string? InstructorName { get; set; }
    public string? ExternalInstructor { get; set; }
    public Guid AreaId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public int Duration { get; set; }
    public string? Location { get; set; }
    public int? MaxParticipants { get; set; }
    public int CurrentParticipants { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? MaterialUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TrainingParticipantDto> Participants { get; set; } = new();
}

public class TrainingParticipantDto
{
    public Guid ParticipantId { get; set; }
    public Guid TrainingId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string AttendanceStatus { get; set; } = string.Empty;
    public decimal? Score { get; set; }
    public bool? Passed { get; set; }
    public string? CertificateUrl { get; set; }
    public string? Comments { get; set; }
    public DateTime RegisteredAt { get; set; }
    public DateTime? AttendanceMarkedAt { get; set; }
}

public class AttendanceDto
{
    public Guid ParticipantId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string AttendanceStatus { get; set; } = string.Empty;
    public decimal? Score { get; set; }
    public bool? Passed { get; set; }
    public string? Comments { get; set; }
}