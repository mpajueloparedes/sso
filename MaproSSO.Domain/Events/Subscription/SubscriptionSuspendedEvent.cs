using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Events.Subscription
{
    public class SubscriptionSuspendedEvent : IDomainEvent
    {
        public Guid SubscriptionId { get; }
        public Guid TenantId { get; }
        public string Reason { get; }
        public DateTime GracePeriodEndDate { get; }
        public DateTime OccurredOn { get; }

        public SubscriptionSuspendedEvent(Guid subscriptionId, Guid tenantId, string reason, DateTime gracePeriodEndDate)
        {
            SubscriptionId = subscriptionId;
            TenantId = tenantId;
            Reason = reason;
            GracePeriodEndDate = gracePeriodEndDate;
            OccurredOn = DateTime.UtcNow;
        }
    }
}