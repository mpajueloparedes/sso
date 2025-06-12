using MediatR;
using MaproSSO.Application.Features.Trainings.DTOs;

namespace MaproSSO.Application.Features.Trainings.Queries;

public record GetTrainingByIdQuery : IRequest<TrainingDto?>
{
    public Guid TrainingId { get; init; }
}

public record GetTrainingsByAreaQuery : IRequest<List<TrainingDto>>
{
    public Guid AreaId { get; init; }
    public string? Status { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public record GetTrainingParticipantsQuery : IRequest<List<TrainingParticipantDto>>
{
    public Guid TrainingId { get; init; }
    public string? AttendanceStatus { get; init; }
}

public record GetUserTrainingsQuery : IRequest<List<TrainingDto>>
{
    public Guid UserId { get; init; }
    public string? Status { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public record GetTrainingStatisticsQuery : IRequest<TrainingStatisticsDto>
{
    public Guid? AreaId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}

public class TrainingStatisticsDto
{
    public int TotalTrainings { get; set; }
    public int CompletedTrainings { get; set; }
    public int ScheduledTrainings { get; set; }
    public int InProgressTrainings { get; set; }
    public int CancelledTrainings { get; set; }
    public int TotalParticipants { get; set; }
    public int PresentParticipants { get; set; }
    public int AbsentParticipants { get; set; }
    public decimal AttendanceRate { get; set; }
    public decimal AverageScore { get; set; }
    public int CertificatesIssued { get; set; }
    public Dictionary<string, int> TrainingsByType { get; set; } = new();
    public Dictionary<string, int> TrainingsByMode { get; set; } = new();
    public Dictionary<string, decimal> AttendanceByArea { get; set; } = new();
}