using MediatR;
using MaproSSO.Application.Features.Audit.DTOs;

namespace MaproSSO.Application.Features.Audit.Queries;

public record GetAuditLogsQuery : IRequest<List<AuditLogDto>>
{
    public Guid? TenantId { get; init; }
    public Guid? UserId { get; init; }
    public string? Action { get; init; }
    public string? EntityType { get; init; }
    public string? EntityId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public bool? Success { get; init; }
    public string? IpAddress { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

public record GetAccessLogsQuery : IRequest<List<AccessLogDto>>
{
    public Guid? TenantId { get; init; }
    public Guid? UserId { get; init; }
    public string? Action { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public bool? Success { get; init; }
    public string? IpAddress { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

public record GetAuditStatisticsQuery : IRequest<AuditStatisticsDto>
{
    public Guid? TenantId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}

public record GetUserActivityQuery : IRequest<List<AuditLogDto>>
{
    public Guid UserId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public record GetEntityAuditHistoryQuery : IRequest<List<AuditLogDto>>
{
    public string EntityType { get; init; } = string.Empty;
    public string EntityId { get; init; } = string.Empty;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public record GetSecurityAlertsQuery : IRequest<List<SecurityAlertDto>>
{
    public Guid? TenantId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string? Severity { get; init; }
}