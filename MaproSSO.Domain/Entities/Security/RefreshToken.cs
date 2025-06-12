using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.Security
{
    public class RefreshToken : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string Token { get; private set; }
        public string DeviceInfo { get; private set; }
        public string IpAddress { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public Guid? RevokedBy { get; private set; }
        public string RevokedReason { get; private set; }
        public string ReplacedByToken { get; private set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt.HasValue;
        public bool IsActive => !IsRevoked && !IsExpired;

        private RefreshToken() { }

        public static RefreshToken Create(
            Guid userId,
            string token,
            string deviceInfo,
            string ipAddress,
            int expirationDays = 7)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new DomainException("El token es requerido");

            if (expirationDays <= 0)
                throw new DomainException("Los días de expiración deben ser mayor a cero");

            return new RefreshToken
            {
                UserId = userId,
                Token = token,
                DeviceInfo = deviceInfo,
                IpAddress = ipAddress,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(expirationDays)
            };
        }

        public void Revoke(string reason, Guid revokedBy, string replacedByToken = null)
        {
            if (IsRevoked)
                throw new BusinessRuleValidationException("El token ya está revocado");

            RevokedAt = DateTime.UtcNow;
            RevokedBy = revokedBy;
            RevokedReason = reason;
            ReplacedByToken = replacedByToken;
        }
    }
}