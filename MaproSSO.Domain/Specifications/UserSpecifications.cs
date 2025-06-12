using MaproSSO.Domain.Entities.Security;

namespace MaproSSO.Domain.Specifications
{
    public class ActiveUsersSpecification : BaseSpecification<User>
    {
        public ActiveUsersSpecification(Guid tenantId)
            : base(u => u.TenantId == tenantId && u.IsActive && !u.IsDeleted)
        {
            AddInclude(u => u.UserRoles);
            ApplyOrderBy(u => u.LastName);
        }
    }

    public class UserByEmailSpecification : BaseSpecification<User>
    {
        public UserByEmailSpecification(string email)
            : base(u => u.NormalizedEmail == email.ToUpperInvariant())
        {
            AddInclude(u => u.UserRoles);
        }
    }

    public class UsersWithRoleSpecification : BaseSpecification<User>
    {
        public UsersWithRoleSpecification(Guid tenantId, string roleName)
            : base(u => u.TenantId == tenantId &&
                       u.UserRoles.Any(ur => ur.Role.NormalizedRoleName == roleName.ToUpperInvariant()))
        {
            AddInclude("UserRoles.Role");
        }
    }
}