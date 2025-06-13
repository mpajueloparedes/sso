using MediatR;
using MaproSSO.Application.Features.Audit.DTOs;

namespace MaproSSO.Application.Features.Audit.Commands;

public record CreateAuditLogCommand : IRequest<AuditLogDto>
{
    public Guid? TenantId { get; init; }
    public Guid? UserId { get; init; }
    public string? UserName { get; init; }
    public string Action { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public string? EntityId { get; init; }
    public object? OldValues { get; init; }
    public object? NewValues { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
    public int? Duration { get; init; }
    public bool Success { get; init; } = true;
    public string? ErrorMessage { get; init; }
}

public record CreateAccessLogCommand : IRequest<AccessLogDto>
{
    public Guid? TenantId { get; init; }
    public Guid? UserId { get; init; }
    public string? UserName { get; init; }
    public string Action { get; init; } = string.Empty;
    public string? IpAddress { get; init; }
    public string? Location { get; init; }
    public string? DeviceInfo { get; init; }
    public string? Browser { get; init; }
    public string? OperatingSystem { get; init; }
    public bool Success { get; init; } = true;
    public string? FailureReason { get; init; }
}

public record CleanupOldLogsCommand : IRequest<int>
{
    public DateTime OlderThan { get; init; }
    public string? LogType { get; init; } // AuditLog, AccessLog, or null for both
}