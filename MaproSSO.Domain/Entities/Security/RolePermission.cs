using MaproSSO.Domain.Common;
using System.Security;

namespace MaproSSO.Domain.Entities.Security
{
    public class RolePermission : BaseEntity
    {
        public Guid RoleId { get; private set; }
        public Guid PermissionId { get; private set; }
        public DateTime GrantedAt { get; private set; }
        public Guid GrantedBy { get; private set; }

        public virtual Permission Permission { get; private set; }

        private RolePermission() { }

        public RolePermission(Guid roleId, Guid permissionId, Guid grantedBy)
        {
            RoleId = roleId;
            PermissionId = permissionId;
            GrantedAt = DateTime.UtcNow;
            GrantedBy = grantedBy;
        }
    }
}