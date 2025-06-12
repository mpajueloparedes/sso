using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.Subscription
{
    public interface ISubscriptionRepository : IRepository<Subscription>
    {
        Task<Subscription> GetActiveByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Subscription>> GetExpiringSubscriptionsAsync(int daysBeforeExpiration);
        Task<IEnumerable<Subscription>> GetSubscriptionsToRenewAsync();
        Task<bool> HasActiveSubscriptionAsync(Guid tenantId);
        Task<int> GetActiveSubscriptionsCountAsync();
        Task<decimal> GetMonthlyRecurringRevenueAsync();
    }
}