namespace MaproSSO.Application.Features.Audit.DTOs;

public class AuditLogDto
{
    public Guid AuditId { get; set; }
    public Guid? TenantId { get; set; }
    public string? TenantName { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
    public int? Duration { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object>? ChangedProperties { get; set; }
}

public class AccessLogDto
{
    public Guid LogId { get; set; }
    public Guid? TenantId { get; set; }
    public string? TenantName { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? Location { get; set; }
    public string? DeviceInfo { get; set; }
    public string? Browser { get; set; }
    public string? OperatingSystem { get; set; }
    public bool Success { get; set; }
    public string? FailureReason { get; set; }
    public DateTime Timestamp { get; set; }
}

public class AuditStatisticsDto
{
    public int TotalAuditLogs { get; set; }
    public int TotalAccessLogs { get; set; }
    public int SuccessfulOperations { get; set; }
    public int FailedOperations { get; set; }
    public int UniqueUsers { get; set; }
    public int LoginAttempts { get; set; }
    public int FailedLogins { get; set; }
    public decimal SuccessRate { get; set; }
    public Dictionary<string, int> ActionsByType { get; set; } = new();
    public Dictionary<string, int> AccessByHour { get; set; } = new();
    public Dictionary<string, int> TopUsers { get; set; } = new();
    public Dictionary<string, int> TopIpAddresses { get; set; } = new();
    public Dictionary<string, int> EntityChanges { get; set; } = new();
    public List<SecurityAlertDto> SecurityAlerts { get; set; } = new();
}

public class SecurityAlertDto
{
    public string AlertType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Count { get; set; }
    public DateTime LastOccurrence { get; set; }
    public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
}