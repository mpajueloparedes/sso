using MediatR;
using Microsoft.Extensions.Logging;
using MaproSSO.Application.Common.Interfaces;
using System.Reflection;

namespace MaproSSO.Application.Common.Behaviours;

public class AuditBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<AuditBehaviour<TRequest, TResponse>> _logger;
    private readonly IAuditService _auditService;

    public AuditBehaviour(ILogger<AuditBehaviour<TRequest, TResponse>> logger, IAuditService auditService)
    {
        _logger = logger;
        _auditService = auditService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var startTime = DateTime.UtcNow;
        var success = true;
        string? errorMessage = null;

        try
        {
            _logger.LogInformation("MaproSSO Request: {Name} {@Request}", requestName, request);

            var response = await next();

            var duration = (int)(DateTime.UtcNow - startTime).TotalMilliseconds;

            // Log audit entry for commands (not queries)
            if (IsCommandRequest(requestName))
            {
                await LogAuditEntry(request, requestName, duration, success, errorMessage);
            }

            _logger.LogInformation("MaproSSO Request completed: {Name} in {Duration}ms", requestName, duration);

            return response;
        }
        catch (Exception ex)
        {
            success = false;
            errorMessage = ex.Message;
            var duration = (int)(DateTime.UtcNow - startTime).TotalMilliseconds;

            if (IsCommandRequest(requestName))
            {
                await LogAuditEntry(request, requestName, duration, success, errorMessage);
            }

            _logger.LogError(ex, "MaproSSO Request failed: {Name}", requestName);
            throw;
        }
    }

    private static bool IsCommandRequest(string requestName)
    {
        return requestName.EndsWith("Command");
    }

    private async Task LogAuditEntry(TRequest request, string requestName, int duration, bool success, string? errorMessage)
    {
        try
        {
            var entityType = ExtractEntityType(requestName);
            var entityId = ExtractEntityId(request);
            var action = ExtractAction(requestName);

            await _auditService.LogAsync(
                action: action,
                entityType: entityType,
                entityId: entityId,
                oldValues: null, // Could be enhanced to capture old values
                newValues: request,
                success: success,
                errorMessage: errorMessage
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log audit entry for request {RequestName}", requestName);
        }
    }

    private static string ExtractEntityType(string requestName)
    {
        // Extract entity type from request name
        // Example: CreateInspectionCommand -> Inspection
        var parts = requestName.Replace("Command", "").Split(new[] { "Create", "Update", "Delete", "Get" }, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0] : "Unknown";
    }

    private static string? ExtractEntityId(TRequest request)
    {
        // Try to extract ID from request using reflection
        var idProperty = typeof(TRequest).GetProperties()
            .FirstOrDefault(p => p.Name.EndsWith("Id") && p.PropertyType == typeof(Guid));

        if (idProperty != null)
        {
            var value = idProperty.GetValue(request);
            return value?.ToString();
        }

        return null;
    }

    private static string ExtractAction(string requestName)
    {
        if (requestName.StartsWith("Create")) return "Create";
        if (requestName.StartsWith("Update")) return "Update";
        if (requestName.StartsWith("Delete")) return "Delete";
        if (requestName.StartsWith("Upload")) return "Upload";
        if (requestName.StartsWith("Download")) return "Download";
        return "Execute";
    }
}