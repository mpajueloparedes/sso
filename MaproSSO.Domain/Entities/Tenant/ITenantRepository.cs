using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.Tenant
{
    public interface ITenantRepository : IRepository<Tenant>
    {
        Task<Tenant> GetByTaxIdAsync(string taxId);
        Task<bool> ExistsByTaxIdAsync(string taxId);
        Task<bool> ExistsByEmailAsync(string email);
        Task<IEnumerable<Tenant>> GetActiveTenantsAsync();
        Task<int> GetActiveTenantsCountAsync();
    }
}