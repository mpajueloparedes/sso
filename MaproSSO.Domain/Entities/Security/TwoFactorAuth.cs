using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.Security
{
    public class TwoFactorAuth : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string SecretKey { get; private set; }
        public string QrCodeUrl { get; private set; }
        public List<string> BackupCodes { get; private set; }
        public DateTime EnabledAt { get; private set; }
        public DateTime? LastUsedAt { get; private set; }

        private TwoFactorAuth() { }

        public static TwoFactorAuth Create(Guid userId, string secretKey, string qrCodeUrl)
        {
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new DomainException("La clave secreta es requerida");

            if (string.IsNullOrWhiteSpace(qrCodeUrl))
                throw new DomainException("La URL del código QR es requerida");

            var auth = new TwoFactorAuth
            {
                UserId = userId,
                SecretKey = secretKey,
                QrCodeUrl = qrCodeUrl,
                BackupCodes = GenerateBackupCodes(),
                EnabledAt = DateTime.UtcNow
            };

            return auth;
        }

        private static List<string> GenerateBackupCodes()
        {
            var codes = new List<string>();
            var random = new Random();

            for (int i = 0; i < 10; i++)
            {
                codes.Add(random.Next(100000, 999999).ToString());
            }

            return codes;
        }

        public bool UseBackupCode(string code)
        {
            if (BackupCodes == null || !BackupCodes.Contains(code))
                return false;

            BackupCodes.Remove(code);
            LastUsedAt = DateTime.UtcNow;
            return true;
        }

        public void RecordUsage()
        {
            LastUsedAt = DateTime.UtcNow;
        }

        public void RegenerateBackupCodes()
        {
            BackupCodes = GenerateBackupCodes();
        }
    }
}