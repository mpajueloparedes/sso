using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.Security
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByUsernameAsync(string username, Guid tenantId);
        Task<User> GetByEmailAsync(string email);
        Task<bool> ExistsByUsernameAsync(string username, Guid tenantId);
        Task<bool> ExistsByEmailAsync(string email);
        Task<IEnumerable<User>> GetActiveUsersByTenantAsync(Guid tenantId);
        Task<int> GetActiveUsersCountByTenantAsync(Guid tenantId);
        Task<IEnumerable<User>> GetUsersWithRoleAsync(Guid tenantId, string roleName);
    }
}