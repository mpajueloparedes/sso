using MaproSSO.Domain.Common;
using System.Data;

namespace MaproSSO.Domain.Entities.Security
{
    public class UserRole : BaseEntity
    {
        public Guid UserId { get; private set; }
        public Guid RoleId { get; private set; }
        public DateTime AssignedAt { get; private set; }
        public Guid AssignedBy { get; private set; }

        public virtual Role Role { get; private set; }

        private UserRole() { }

        public UserRole(Guid userId, Guid roleId, Guid assignedBy)
        {
            UserId = userId;
            RoleId = roleId;
            AssignedAt = DateTime.UtcNow;
            AssignedBy = assignedBy;
        }
    }
}