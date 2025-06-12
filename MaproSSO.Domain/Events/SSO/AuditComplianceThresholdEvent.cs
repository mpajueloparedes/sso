using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Events.SSO
{
    public class AuditComplianceThresholdEvent : IDomainEvent
    {
        public Guid AuditId { get; }
        public Guid TenantId { get; }
        public string AuditCode { get; }
        public decimal CompliancePercentage { get; }
        public string AlertLevel { get; } // Critical, Warning, Info
        public DateTime OccurredOn { get; }

        public AuditComplianceThresholdEvent(
            Guid auditId,
            Guid tenantId,
            string auditCode,
            decimal compliancePercentage,
            string alertLevel)
        {
            AuditId = auditId;
            TenantId = tenantId;
            AuditCode = auditCode;
            CompliancePercentage = compliancePercentage;
            AlertLevel = alertLevel;
            OccurredOn = DateTime.UtcNow;
        }
    }
}