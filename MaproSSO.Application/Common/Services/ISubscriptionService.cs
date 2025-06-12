using System;
using System.Threading.Tasks;
using MaproSSO.Application.Common.Models;
using MaproSSO.Domain.Enums;

namespace MaproSSO.Application.Common.Services
{
    public interface ISubscriptionService
    {
        Task<Result<SubscriptionDto>> CreateTrialSubscriptionAsync(Guid tenantId, Guid planId);
        Task<Result<SubscriptionDto>> ActivateSubscriptionAsync(Guid subscriptionId, PaymentDto payment);
        Task<Result<SubscriptionDto>> RenewSubscriptionAsync(Guid subscriptionId, PaymentDto payment);
        Task<Result<SubscriptionDto>> ChangePlanAsync(Guid subscriptionId, Guid newPlanId);
        Task<Result> CancelSubscriptionAsync(Guid subscriptionId, string reason);
        Task<Result> SuspendSubscriptionAsync(Guid subscriptionId, string reason);
        Task CheckAndProcessExpiredSubscriptionsAsync();
        Task SendExpirationNotificationsAsync();
    }

    public class SubscriptionDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid PlanId { get; set; }
        public string PlanName { get; set; }
        public BillingCycle BillingCycle { get; set; }
        public SubscriptionStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? NextBillingDate { get; set; }
        public bool AutoRenew { get; set; }
        public int DaysRemaining { get; set; }
        public decimal CompliancePercentage { get; set; }
    }

    public class PaymentDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
    }
}
