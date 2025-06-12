using System.Collections.Generic;

namespace MaproSSO.Application.Features.Auth.Commands.EnableTwoFactor
{
    public class EnableTwoFactorResponse
    {
        public string SecretKey { get; set; }
        public string QrCodeUrl { get; set; }
        public List<string> BackupCodes { get; set; }
        public bool Enabled { get; set; }
    }
}