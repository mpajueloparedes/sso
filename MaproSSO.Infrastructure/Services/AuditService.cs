using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Features.Audit.Commands;
using UAParser;
using System.Net.Http;

namespace MaproSSO.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuditService> _logger;

    public AuditService(
        IMediator mediator,
        ICurrentUserService currentUserService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuditService> logger)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task LogAsync(string action, string entityType, string? entityId = null, object? oldValues = null, object? newValues = null, bool success = true, string? errorMessage = null)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var ipAddress = GetClientIpAddress(httpContext);
            var userAgent = httpContext?.Request.Headers["User-Agent"].ToString();

            var command = new CreateAuditLogCommand
            {
                TenantId = _currentUserService.TenantId,
                UserId = _currentUserService.UserId,
                UserName = _currentUserService.UserName,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                OldValues = oldValues,
                NewValues = newValues,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Success = success,
                ErrorMessage = errorMessage
            };

            await _mediator.Send(command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging audit entry for action {Action} on {EntityType}", action, entityType);
        }
    }

    public async Task LogAccessAsync(string action, bool success = true, string? failureReason = null)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var ipAddress = GetClientIpAddress(httpContext);
            var userAgent = httpContext?.Request.Headers["User-Agent"].ToString();

            var deviceInfo = ParseDeviceInfo(userAgent);

            var command = new CreateAccessLogCommand
            {
                TenantId = _currentUserService.TenantId,
                UserId = _currentUserService.UserId,
                UserName = _currentUserService.UserName,
                Action = action,
                IpAddress = ipAddress,
                DeviceInfo = deviceInfo.Device,
                Browser = deviceInfo.Browser,
                OperatingSystem = deviceInfo.OS,
                Success = success,
                FailureReason = failureReason
            };

            await _mediator.Send(command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging access entry for action {Action}", action);
        }
    }

    public async Task LogUserActivityAsync(Guid userId, string action, string details)
    {
        try
        {
            await LogAsync(action, "UserActivity", userId.ToString(), null, new { Details = details });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging user activity for user {UserId}", userId);
        }
    }

    public async Task CleanupOldLogsAsync(DateTime olderThan)
    {
        try
        {
            var command = new CleanupOldLogsCommand { OlderThan = olderThan };
            await _mediator.Send(command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up old audit logs");
        }
    }

    private string? GetClientIpAddress(HttpContext? httpContext)
    {
        if (httpContext == null) return null;

        var ipAddress = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        }
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
        }

        return ipAddress;
    }

    private (string Device, string Browser, string OS) ParseDeviceInfo(string? userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return ("Unknown", "Unknown", "Unknown");

        try
        {
            var parser = Parser.GetDefault();
            var clientInfo = parser.Parse(userAgent);

            return (
                Device: $"{clientInfo.Device.Family} {clientInfo.Device.Model}".Trim(),
                Browser: $"{clientInfo.UserAgent.Family} {clientInfo.UserAgent.Major}".Trim(),
                OS: $"{clientInfo.OS.Family} {clientInfo.OS.Major}".Trim()
            );
        }
        catch
        {
            return ("Unknown", "Unknown", "Unknown");
        }
    }
}