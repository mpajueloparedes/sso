using MediatR;
using MaproSSO.Application.Features.Trainings.DTOs;

namespace MaproSSO.Application.Features.Trainings.Commands;

public record CreateTrainingCommand : IRequest<TrainingDto>
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string TrainingType { get; init; } = string.Empty;
    public string Mode { get; init; } = string.Empty;
    public Guid? InstructorUserId { get; init; }
    public string? ExternalInstructor { get; init; }
    public Guid AreaId { get; init; }
    public DateTime ScheduledDate { get; init; }
    public int Duration { get; init; }
    public string? Location { get; init; }
    public int? MaxParticipants { get; init; }
    public string? MaterialUrl { get; init; }
}

public record UpdateTrainingCommand : IRequest<TrainingDto>
{
    public Guid TrainingId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string TrainingType { get; init; } = string.Empty;
    public string Mode { get; init; } = string.Empty;
    public Guid? InstructorUserId { get; init; }
    public string? ExternalInstructor { get; init; }
    public DateTime ScheduledDate { get; init; }
    public int Duration { get; init; }
    public string? Location { get; init; }
    public int? MaxParticipants { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? MaterialUrl { get; init; }
}

public record RegisterParticipantCommand : IRequest<TrainingParticipantDto>
{
    public Guid TrainingId { get; init; }
    public Guid UserId { get; init; }
}

public record RegisterMultipleParticipantsCommand : IRequest<List<TrainingParticipantDto>>
{
    public Guid TrainingId { get; init; }
    public List<Guid> UserIds { get; init; } = new();
}

public record TakeAttendanceCommand : IRequest<List<TrainingParticipantDto>>
{
    public Guid TrainingId { get; init; }
    public List<AttendanceRecordDto> AttendanceRecords { get; init; } = new();
}

public record CompleteTrainingCommand : IRequest<TrainingDto>
{
    public Guid TrainingId { get; init; }
}

public class AttendanceRecordDto
{
    public Guid UserId { get; set; }
    public string AttendanceStatus { get; set; } = string.Empty;
    public decimal? Score { get; set; }
    public bool? Passed { get; set; }
    public string? Comments { get; set; }
}