using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.Subscription
{
    public class SubscriptionHistory : BaseEntity
    {
        public Guid SubscriptionId { get; private set; }
        public string Action { get; private set; }
        public Guid? FromPlanId { get; private set; }
        public Guid? ToPlanId { get; private set; }
        public string FromBillingCycle { get; private set; }
        public string ToBillingCycle { get; private set; }
        public string Reason { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid CreatedBy { get; private set; }

        private SubscriptionHistory() { }

        public static SubscriptionHistory Create(
            Guid subscriptionId,
            string action,
            Guid? fromPlanId,
            Guid? toPlanId,
            string fromBillingCycle,
            string toBillingCycle,
            Guid createdBy,
            string reason = null)
        {
            return new SubscriptionHistory
            {
                SubscriptionId = subscriptionId,
                Action = action,
                FromPlanId = fromPlanId,
                ToPlanId = toPlanId,
                FromBillingCycle = fromBillingCycle,
                ToBillingCycle = toBillingCycle,
                Reason = reason,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            };
        }
    }
}