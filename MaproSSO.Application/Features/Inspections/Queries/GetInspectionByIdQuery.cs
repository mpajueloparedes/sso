using MediatR;
using MaproSSO.Application.Features.Inspections.DTOs;

namespace MaproSSO.Application.Features.Inspections.Queries;

public record GetInspectionByIdQuery : IRequest<InspectionDto?>
{
    public Guid InspectionId { get; init; }
}

public record GetInspectionsByAreaQuery : IRequest<List<InspectionDto>>
{
    public Guid AreaId { get; init; }
    public string? Status { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public record GetInspectionsByProgramQuery : IRequest<List<InspectionDto>>
{
    public Guid ProgramId { get; init; }
    public string? Status { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public record GetInspectionProgramsQuery : IRequest<List<InspectionProgramDto>>
{
    public int? Year { get; init; }
    public bool? IsActive { get; init; }
}

public record GetInspectionStatisticsQuery : IRequest<InspectionStatisticsDto>
{
    public Guid? AreaId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}

public class InspectionStatisticsDto
{
    public int TotalInspections { get; set; }
    public int CompletedInspections { get; set; }
    public int PendingInspections { get; set; }
    public int InProgressInspections { get; set; }
    public decimal CompletionRate { get; set; }
    public int TotalObservations { get; set; }
    public int CriticalObservations { get; set; }
    public int HighObservations { get; set; }
    public int MediumObservations { get; set; }
    public int LowObservations { get; set; }
}