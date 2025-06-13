using MaproSSO.Domain.Common;
using MaproSSO.Domain.Entities.Tenant;

namespace MaproSSO.Domain.Entities.Audit;

public class AccessLog : BaseEntity
{
    public Guid LogId { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string Action { get; set; } = string.Empty; // Login, Logout, FailedLogin, PasswordChange
    public string? IpAddress { get; set; }
    public string? Location { get; set; }
    public string? DeviceInfo { get; set; }
    public string? Browser { get; set; }
    public string? OperatingSystem { get; set; }
    public bool Success { get; set; } = true;
    public string? FailureReason { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Tenant.Tenant? Tenant { get; set; }
    public virtual Security.User? User { get; set; }
}
