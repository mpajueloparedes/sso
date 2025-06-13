using MaproSSO.Domain.Common;
using MaproSSO.Domain.Entities.Tenant;

namespace MaproSSO.Domain.Entities.AuditLogging; // Changed namespace

public class AuditLog : BaseEntity
{
    public Guid AuditId { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int? Duration { get; set; }
    public bool Success { get; set; } = true;
    public string? ErrorMessage { get; set; }

    // Navigation properties
    public virtual Tenant.Tenant? Tenant { get; set; }
    public virtual Security.User? User { get; set; }
}
