using MaproSSO.Domain.Common;
using MaproSSO.Domain.Exceptions;

namespace MaproSSO.Domain.Entities.Security
{
    public class Role : BaseEntity
    {
        private readonly List<RolePermission> _rolePermissions = new();

        public string RoleName { get; private set; }
        public string NormalizedRoleName { get; private set; }
        public string Description { get; private set; }
        public bool IsSystemRole { get; private set; }
        public Guid? TenantId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

        private Role() { }

        public static Role CreateSystemRole(string roleName, string description = null)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new DomainException("El nombre del rol es requerido");

            return new Role
            {
                RoleName = roleName,
                NormalizedRoleName = roleName.ToUpperInvariant(),
                Description = description,
                IsSystemRole = true,
                TenantId = null,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static Role CreateTenantRole(Guid tenantId, string roleName, string description = null)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new DomainException("El nombre del rol es requerido");

            return new Role
            {
                RoleName = roleName,
                NormalizedRoleName = roleName.ToUpperInvariant(),
                Description = description,
                IsSystemRole = false,
                TenantId = tenantId,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(string roleName, string description)
        {
            if (IsSystemRole)
                throw new BusinessRuleValidationException("Los roles del sistema no pueden ser modificados");

            RoleName = roleName;
            NormalizedRoleName = roleName.ToUpperInvariant();
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AssignPermission(Guid permissionId, Guid grantedBy)
        {
            if (_rolePermissions.Any(rp => rp.PermissionId == permissionId))
                throw new BusinessRuleValidationException("El permiso ya está asignado al rol");

            _rolePermissions.Add(new RolePermission(Id, permissionId, grantedBy));
        }

        public void RemovePermission(Guid permissionId)
        {
            var rolePermission = _rolePermissions.FirstOrDefault(rp => rp.PermissionId == permissionId);
            if (rolePermission != null)
            {
                _rolePermissions.Remove(rolePermission);
            }
        }

        public bool HasPermission(string permissionCode)
        {
            return _rolePermissions.Any(rp => rp.Permission?.PermissionCode == permissionCode);
        }
    }
}