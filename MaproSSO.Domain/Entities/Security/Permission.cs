using MaproSSO.Domain.Common;
using MaproSSO.Domain.Exceptions;

namespace MaproSSO.Domain.Entities.Security
{
    public class Permission : BaseEntity
    {
        public string PermissionName { get; private set; }
        public string PermissionCode { get; private set; }
        public string Module { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Permission() { }

        public static Permission Create(
            string permissionName,
            string permissionCode,
            string module,
            string description = null)
        {
            if (string.IsNullOrWhiteSpace(permissionName))
                throw new DomainException("El nombre del permiso es requerido");

            if (string.IsNullOrWhiteSpace(permissionCode))
                throw new DomainException("El código del permiso es requerido");

            if (string.IsNullOrWhiteSpace(module))
                throw new DomainException("El módulo es requerido");

            return new Permission
            {
                PermissionName = permissionName,
                PermissionCode = permissionCode.ToLowerInvariant(),
                Module = module,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}