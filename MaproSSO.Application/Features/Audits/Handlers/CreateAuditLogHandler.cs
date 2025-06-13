using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Features.Audit.Commands;
using MaproSSO.Application.Features.Audit.DTOs;
using MaproSSO.Domain.Entities.Audit;
using System.Text.Json;
using MaproSSO.Application.Features.Audit.Queries;

namespace MaproSSO.Application.Features.Audit.Handlers;

public class CreateAuditLogHandler : IRequestHandler<CreateAuditLogCommand, AuditLogDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateAuditLogHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<AuditLogDto> Handle(CreateAuditLogCommand request, CancellationToken cancellationToken)
    {
        var auditLog = new AuditLog
        {
            AuditId = Guid.NewGuid(),
            TenantId = request.TenantId,
            UserId = request.UserId,
            UserName = request.UserName,
            Action = request.Action,
            EntityType = request.EntityType,
            EntityId = request.EntityId,
            OldValues = request.OldValues != null ? JsonSerializer.Serialize(request.OldValues) : null,
            NewValues = request.NewValues != null ? JsonSerializer.Serialize(request.NewValues) : null,
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent,
            Duration = request.Duration,
            Success = request.Success,
            ErrorMessage = request.ErrorMessage,
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync(cancellationToken);

        var result = await _context.AuditLogs
            .Include(a => a.Tenant)
            .Include(a => a.User)
            .FirstAsync(a => a.AuditId == auditLog.AuditId, cancellationToken);

        return _mapper.Map<AuditLogDto>(result);
    }
}

public class CreateAccessLogHandler : IRequestHandler<CreateAccessLogCommand, AccessLogDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateAccessLogHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<AccessLogDto> Handle(CreateAccessLogCommand request, CancellationToken cancellationToken)
    {
        var accessLog = new AccessLog
        {
            LogId = Guid.NewGuid(),
            TenantId = request.TenantId,
            UserId = request.UserId,
            UserName = request.UserName,
            Action = request.Action,
            IpAddress = request.IpAddress,
            Location = request.Location,
            DeviceInfo = request.DeviceInfo,
            Browser = request.Browser,
            OperatingSystem = request.OperatingSystem,
            Success = request.Success,
            FailureReason = request.FailureReason,
            Timestamp = DateTime.UtcNow
        };

        _context.AccessLogs.Add(accessLog);
        await _context.SaveChangesAsync(cancellationToken);

        var result = await _context.AccessLogs
            .Include(a => a.Tenant)
            .Include(a => a.User)
            .FirstAsync(a => a.LogId == accessLog.LogId, cancellationToken);

        return _mapper.Map<AccessLogDto>(result);
    }
}

public class GetAuditLogsHandler : IRequestHandler<GetAuditLogsQuery, List<AuditLogDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAuditLogsHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<AuditLogDto>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.AuditLogs
            .Include(a => a.Tenant)
            .Include(a => a.User)
            .AsQueryable();

        // Apply filters
        if (request.TenantId.HasValue)
            query = query.Where(a => a.TenantId == request.TenantId.Value);

        if (request.UserId.HasValue)
            query = query.Where(a => a.UserId == request.UserId.Value);

        if (!string.IsNullOrEmpty(request.Action))
            query = query.Where(a => a.Action == request.Action);

        if (!string.IsNullOrEmpty(request.EntityType))
            query = query.Where(a => a.EntityType == request.EntityType);

        if (!string.IsNullOrEmpty(request.EntityId))
            query = query.Where(a => a.EntityId == request.EntityId);

        if (request.FromDate.HasValue)
            query = query.Where(a => a.Timestamp >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(a => a.Timestamp <= request.ToDate.Value);

        if (request.Success.HasValue)
            query = query.Where(a => a.Success == request.Success.Value);

        if (!string.IsNullOrEmpty(request.IpAddress))
            query = query.Where(a => a.IpAddress == request.IpAddress);

        // Pagination and ordering
        var auditLogs = await query
            .OrderByDescending(a => a.Timestamp)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<AuditLogDto>>(auditLogs);
    }
}

public class GetAuditStatisticsHandler : IRequestHandler<GetAuditStatisticsQuery, AuditStatisticsDto>
{
    private readonly IApplicationDbContext _context;

    public GetAuditStatisticsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AuditStatisticsDto> Handle(GetAuditStatisticsQuery request, CancellationToken cancellationToken)
    {
        var fromDate = request.FromDate ?? DateTime.UtcNow.AddDays(-30);
        var toDate = request.ToDate ?? DateTime.UtcNow;

        var auditQuery = _context.AuditLogs.AsQueryable();
        var accessQuery = _context.AccessLogs.AsQueryable();

        if (request.TenantId.HasValue)
        {
            auditQuery = auditQuery.Where(a => a.TenantId == request.TenantId.Value);
            accessQuery = accessQuery.Where(a => a.TenantId == request.TenantId.Value);
        }

        auditQuery = auditQuery.Where(a => a.Timestamp >= fromDate && a.Timestamp <= toDate);
        accessQuery = accessQuery.Where(a => a.Timestamp >= fromDate && a.Timestamp <= toDate);

        var totalAuditLogs = await auditQuery.CountAsync(cancellationToken);
        var totalAccessLogs = await accessQuery.CountAsync(cancellationToken);
        var successfulOperations = await auditQuery.CountAsync(a => a.Success, cancellationToken);
        var failedOperations = totalAuditLogs - successfulOperations;
        var uniqueUsers = await auditQuery.Where(a => a.UserId.HasValue).Select(a => a.UserId).Distinct().CountAsync(cancellationToken);
        var loginAttempts = await accessQuery.CountAsync(a => a.Action == "Login", cancellationToken);
        var failedLogins = await accessQuery.CountAsync(a => a.Action == "Login" && !a.Success, cancellationToken);

        // Actions by type
        var actionsByType = await auditQuery
            .GroupBy(a => a.Action)
            .Select(g => new { Action = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Action, x => x.Count, cancellationToken);

        // Access by hour
        var accessByHour = await accessQuery
            .GroupBy(a => a.Timestamp.Hour)
            .Select(g => new { Hour = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Hour.ToString("00"), x => x.Count, cancellationToken);

        // Top users
        var topUsers = await auditQuery
            .Where(a => a.UserName != null)
            .GroupBy(a => a.UserName)
            .Select(g => new { UserName = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToDictionaryAsync(x => x.UserName!, x => x.Count, cancellationToken);

        // Top IP addresses
        var topIpAddresses = await accessQuery
            .Where(a => a.IpAddress != null)
            .GroupBy(a => a.IpAddress)
            .Select(g => new { IpAddress = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToDictionaryAsync(x => x.IpAddress!, x => x.Count, cancellationToken);

        // Entity changes
        var entityChanges = await auditQuery
            .Where(a => a.Action == "Update" || a.Action == "Create" || a.Action == "Delete")
            .GroupBy(a => a.EntityType)
            .Select(g => new { EntityType = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.EntityType, x => x.Count, cancellationToken);

        // Security alerts
        var securityAlerts = await GenerateSecurityAlerts(request.TenantId, fromDate, toDate, cancellationToken);

        return new AuditStatisticsDto
        {
            TotalAuditLogs = totalAuditLogs,
            TotalAccessLogs = totalAccessLogs,
            SuccessfulOperations = successfulOperations,
            FailedOperations = failedOperations,
            UniqueUsers = uniqueUsers,
            LoginAttempts = loginAttempts,
            FailedLogins = failedLogins,
            SuccessRate = totalAuditLogs > 0 ? Math.Round((decimal)successfulOperations / totalAuditLogs * 100, 2) : 0,
            ActionsByType = actionsByType,
            AccessByHour = accessByHour,
            TopUsers = topUsers,
            TopIpAddresses = topIpAddresses,
            EntityChanges = entityChanges,
            SecurityAlerts = securityAlerts
        };
    }

    private async Task<List<SecurityAlertDto>> GenerateSecurityAlerts(Guid? tenantId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        var alerts = new List<SecurityAlertDto>();

        var accessQuery = _context.AccessLogs.Where(a => a.Timestamp >= fromDate && a.Timestamp <= toDate);
        if (tenantId.HasValue)
            accessQuery = accessQuery.Where(a => a.TenantId == tenantId.Value);

        // Multiple failed login attempts
        var suspiciousIPs = await accessQuery
            .Where(a => a.Action == "Login" && !a.Success)
            .GroupBy(a => a.IpAddress)
            .Where(g => g.Count() >= 5)
            .Select(g => new { IpAddress = g.Key, Count = g.Count(), LastAttempt = g.Max(x => x.Timestamp) })
            .ToListAsync(cancellationToken);

        foreach (var suspiciousIP in suspiciousIPs)
        {
            alerts.Add(new SecurityAlertDto
            {
                AlertType = "Multiple Failed Logins",
                Description = $"IP {suspiciousIP.IpAddress} has {suspiciousIP.Count} failed login attempts",
                Count = suspiciousIP.Count,
                LastOccurrence = suspiciousIP.LastAttempt,
                Severity = suspiciousIP.Count >= 10 ? "Critical" : "High"
            });
        }

        // After-hours access
        var afterHoursAccess = await accessQuery
            .Where(a => a.Action == "Login" && a.Success && (a.Timestamp.Hour < 6 || a.Timestamp.Hour > 22))
            .CountAsync(cancellationToken);

        if (afterHoursAccess > 0)
        {
            alerts.Add(new SecurityAlertDto
            {
                AlertType = "After Hours Access",
                Description = $"{afterHoursAccess} successful logins outside business hours",
                Count = afterHoursAccess,
                LastOccurrence = await accessQuery
                    .Where(a => a.Action == "Login" && a.Success && (a.Timestamp.Hour < 6 || a.Timestamp.Hour > 22))
                    .MaxAsync(a => a.Timestamp, cancellationToken),
                Severity = afterHoursAccess > 10 ? "Medium" : "Low"
            });
        }

        return alerts;
    }
}