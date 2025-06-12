using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.Security
{
    public class PasswordHistory : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string PasswordHash { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private PasswordHistory() { }

        public PasswordHistory(Guid userId, string passwordHash)
        {
            UserId = userId;
            PasswordHash = passwordHash;
            CreatedAt = DateTime.UtcNow;
        }
    }
}