using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Events.Subscription
{
    public class SubscriptionRenewedEvent : IDomainEvent
    {
        public Guid SubscriptionId { get; }
        public Guid TenantId { get; }
        public DateTime NewEndDate { get; }
        public DateTime OccurredOn { get; }

        public SubscriptionRenewedEvent(Guid subscriptionId, Guid tenantId, DateTime newEndDate)
        {
            SubscriptionId = subscriptionId;
            TenantId = tenantId;
            NewEndDate = newEndDate;
            OccurredOn = DateTime.UtcNow;
        }
    }
}