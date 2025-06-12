using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Events.Subscription
{
    public class PaymentCompletedEvent : IDomainEvent
    {
        public Guid PaymentId { get; }
        public Guid SubscriptionId { get; }
        public decimal Amount { get; }
        public string Currency { get; }
        public DateTime OccurredOn { get; }

        public PaymentCompletedEvent(Guid paymentId, Guid subscriptionId, decimal amount, string currency)
        {
            PaymentId = paymentId;
            SubscriptionId = subscriptionId;
            Amount = amount;
            Currency = currency;
            OccurredOn = DateTime.UtcNow;
        }
    }
}