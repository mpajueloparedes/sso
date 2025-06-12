using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Events.Tenant
{
    public class TenantDeactivatedEvent : IDomainEvent
    {
        public Guid TenantId { get; }
        public string CompanyName { get; }
        public DateTime OccurredOn { get; }

        public TenantDeactivatedEvent(Guid tenantId, string companyName)
        {
            TenantId = tenantId;
            CompanyName = companyName;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
