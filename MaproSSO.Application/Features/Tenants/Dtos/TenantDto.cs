using MaproSSO.Application.Features.Audit.DTOs;
using MaproSSO.Domain.Enums;

namespace MaproSSO.Application.Features.Tenants.DTOs;

public class TenantDto
{
    public Guid TenantId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? TradeName { get; set; }
    public string TaxId { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
    public string Country { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string? LogoUrl { get; set; }
    public string TimeZone { get; set; } = "America/Lima";
    public string Culture { get; set; } = "es-PE";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }

    // Calculated properties
    public string FullAddress => $"{Address}, {City}, {State}, {Country}";
    public string DisplayName => !string.IsNullOrEmpty(TradeName) ? TradeName : CompanyName;
    public int DaysActive => (DateTime.UtcNow - CreatedAt).Days;

    // Related data
    public SubscriptionDto? CurrentSubscription { get; set; }
    public List<TenantSettingDto> Settings { get; set; } = new();
    public TenantStatisticsDto? Statistics { get; set; }
}

public class TenantListDto
{
    public Guid TenantId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? TradeName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
    public string Country { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string DisplayName => !string.IsNullOrEmpty(TradeName) ? TradeName : CompanyName;

    // Subscription info
    public string? SubscriptionPlan { get; set; }
    public SubscriptionStatus? SubscriptionStatus { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
    public bool IsSubscriptionExpiringSoon => SubscriptionEndDate.HasValue &&
        SubscriptionEndDate.Value.Subtract(DateTime.UtcNow).Days <= 7;
}

public class TenantSummaryDto
{
    public Guid TenantId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? TradeName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; }
    public string DisplayName => !string.IsNullOrEmpty(TradeName) ? TradeName : CompanyName;
}

public class TenantStatisticsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalAreas { get; set; }
    public int TotalInspections { get; set; }
    public int TotalAudits { get; set; }
    public int TotalAccidents { get; set; }
    public int TotalTrainings { get; set; }
    public int TotalDocuments { get; set; }
    public long TotalStorageUsed { get; set; }
    public string TotalStorageFormatted { get; set; } = string.Empty;
    public DateTime LastLoginDate { get; set; }
    public string LastLoginUser { get; set; } = string.Empty;
    public Dictionary<string, int> ModuleUsage { get; set; } = new();
    public Dictionary<string, int> ActivityByMonth { get; set; } = new();
}

public class TenantDashboardDto
{
    public TenantDto Tenant { get; set; } = null!;
    public SubscriptionDto? Subscription { get; set; }
    public TenantStatisticsDto Statistics { get; set; } = null!;
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
    public List<SecurityAlertDto> SecurityAlerts { get; set; } = new();
    public SystemHealthDto SystemHealth { get; set; } = null!;
}

public class RecentActivityDto
{
    public Guid ActivityId { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class SystemHealthDto
{
    public string Status { get; set; } = string.Empty; // Healthy, Warning, Critical
    public decimal OverallScore { get; set; }
    public List<HealthCheckDto> HealthChecks { get; set; } = new();
}

public class HealthCheckDto
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime LastChecked { get; set; }
    public string? ErrorMessage { get; set; }
}