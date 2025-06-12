using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.SSO
{
    public class AreaUser : BaseEntity
    {
        public Guid AreaId { get; private set; }
        public Guid UserId { get; private set; }
        public string Role { get; private set; } // Leader, User
        public DateTime AssignedAt { get; private set; }
        public Guid AssignedBy { get; private set; }

        private AreaUser() { }

        public AreaUser(Guid areaId, Guid userId, string role, Guid assignedBy)
        {
            AreaId = areaId;
            UserId = userId;
            Role = role;
            AssignedAt = DateTime.UtcNow;
            AssignedBy = assignedBy;
        }
    }
}