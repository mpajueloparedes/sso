namespace MaproSSO.Application.Common.Interfaces;

public interface IAuditService
{
    Task LogAsync(string action, string entityType, string? entityId = null, object? oldValues = null, object? newValues = null, bool success = true, string? errorMessage = null);
    Task LogAccessAsync(string action, bool success = true, string? failureReason = null);
    Task LogUserActivityAsync(Guid userId, string action, string details);
    Task CleanupOldLogsAsync(DateTime olderThan);
}