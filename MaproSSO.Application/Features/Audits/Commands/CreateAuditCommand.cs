using MediatR;
using MaproSSO.Application.Features.Audits.DTOs;

namespace MaproSSO.Application.Features.Audits.Commands;

public record CreateAuditCommand : IRequest<AuditDto>
{
    public Guid ProgramId { get; init; }
    public Guid AreaId { get; init; }
    public string AuditType { get; init; } = string.Empty;
    public Guid AuditorUserId { get; init; }
    public DateTime ScheduledDate { get; init; }
}

public record UpdateAuditCommand : IRequest<AuditDto>
{
    public Guid AuditId { get; init; }
    public string AuditType { get; init; } = string.Empty;
    public Guid AuditorUserId { get; init; }
    public DateTime ScheduledDate { get; init; }
    public string Status { get; init; } = string.Empty;
}

public record EvaluateAuditCriteriaCommand : IRequest<AuditEvaluationDto>
{
    public Guid AuditId { get; init; }
    public Guid CriteriaId { get; init; }
    public decimal Score { get; init; }
    public string? Observations { get; init; }
    public bool EvidenceRequired { get; init; }
    public List<string> EvidenceUrls { get; init; } = new();
}

public record CompleteAuditCommand : IRequest<AuditDto>
{
    public Guid AuditId { get; init; }
}