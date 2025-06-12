using MediatR;
using MaproSSO.Application.Features.Audits.DTOs;

namespace MaproSSO.Application.Features.Audits.Queries;

public record GetAuditByIdQuery : IRequest<AuditDto?>
{
    public Guid AuditId { get; init; }
}

public record GetAuditsByProgramQuery : IRequest<List<AuditDto>>
{
    public Guid ProgramId { get; init; }
    public string? Status { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public record GetAuditCategoriesQuery : IRequest<List<AuditCategoryDto>>
{
    public bool? IsActive { get; init; }
}

public record GetAuditStatisticsQuery : IRequest<AuditStatisticsDto>
{
    public Guid? AreaId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}

public class AuditStatisticsDto
{
    public int TotalAudits { get; set; }
    public int CompletedAudits { get; set; }
    public int ScheduledAudits { get; set; }
    public int InProgressAudits { get; set; }
    public decimal AverageCompliancePercentage { get; set; }
    public int TotalEvaluations { get; set; }
    public Dictionary<string, int> AuditsByType { get; set; } = new();
    public Dictionary<string, decimal> ComplianceByCategory { get; set; } = new();
}