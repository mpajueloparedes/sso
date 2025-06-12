using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Events.Tenant
{
    public class TenantCreatedEvent : IDomainEvent
    {
        public Guid TenantId { get; }
        public string CompanyName { get; }
        public string Email { get; }
        public DateTime OccurredOn { get; }

        public TenantCreatedEvent(Guid tenantId, string companyName, string email)
        {
            TenantId = tenantId;
            CompanyName = companyName;
            Email = email;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
