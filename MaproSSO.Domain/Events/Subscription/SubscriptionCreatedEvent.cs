using MaproSSO.Domain.Common;
using MaproSSO.Domain.Enums;

namespace MaproSSO.Domain.Events.Subscription
{
    public class SubscriptionCreatedEvent : IDomainEvent
    {
        public Guid SubscriptionId { get; }
        public Guid TenantId { get; }
        public Guid PlanId { get; }
        public SubscriptionStatus Status { get; }
        public DateTime OccurredOn { get; }

        public SubscriptionCreatedEvent(Guid subscriptionId, Guid tenantId, Guid planId, SubscriptionStatus status)
        {
            SubscriptionId = subscriptionId;
            TenantId = tenantId;
            PlanId = planId;
            Status = status;
            OccurredOn = DateTime.UtcNow;
        }
    }
}