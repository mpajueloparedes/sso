using System;
using System.Threading.Tasks;
using MaproSSO.Domain.Enums;

namespace MaproSSO.Application.Common.Interfaces
{
    public interface ISubscriptionValidationService
    {
        Task<bool> ValidateSubscription(Guid tenantId);
        Task<bool> ValidateFeatureAccess(Guid tenantId, string featureCode);
        Task<int?> GetFeatureLimit(Guid tenantId, string featureCode);
        Task<bool> IncrementFeatureUsage(Guid tenantId, string featureCode, int increment = 1);
        Task<bool> DecrementFeatureUsage(Guid tenantId, string featureCode, int decrement = 1);
        Task<SubscriptionStatus> GetSubscriptionStatus(Guid tenantId);
        Task<FeatureUsageInfo> GetFeatureUsage(Guid tenantId, string featureCode);
    }

    public class FeatureUsageInfo
    {
        public string FeatureCode { get; set; }
        public int CurrentUsage { get; set; }
        public int? Limit { get; set; }
        public int Remaining => Limit.HasValue ? Math.Max(0, Limit.Value - CurrentUsage) : int.MaxValue;
        public decimal UsagePercentage => Limit.HasValue && Limit.Value > 0 ? (CurrentUsage * 100m) / Limit.Value : 0;
        public bool IsAtLimit => Limit.HasValue && CurrentUsage >= Limit.Value;
    }
}
