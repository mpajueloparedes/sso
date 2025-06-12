using MaproSSO.Domain.Common;
using MaproSSO.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaproSSO.Domain.Entities.Security
{
    public class UserSession : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string SessionToken { get; private set; }
        public string DeviceType { get; private set; }
        public string DeviceName { get; private set; }
        public string Browser { get; private set; }
        public string OperatingSystem { get; private set; }
        public string IpAddress { get; private set; }
        public string Location { get; private set; }
        public DateTime StartedAt { get; private set; }
        public DateTime LastActivityAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime? EndedAt { get; private set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool HasEnded => EndedAt.HasValue;
        public bool IsActive => !HasEnded && !IsExpired;

        private UserSession() { }

        public static UserSession Create(
            Guid userId,
            string sessionToken,
            string deviceType,
            string deviceName,
            string browser,
            string operatingSystem,
            string ipAddress,
            string location = null,
            int expirationMinutes = 30)
        {
            if (string.IsNullOrWhiteSpace(sessionToken))
                throw new DomainException("El token de sesión es requerido");

            if (expirationMinutes <= 0)
                throw new DomainException("Los minutos de expiración deben ser mayor a cero");

            return new UserSession
            {
                UserId = userId,
                SessionToken = sessionToken,
                DeviceType = deviceType,
                DeviceName = deviceName,
                Browser = browser,
                OperatingSystem = operatingSystem,
                IpAddress = ipAddress,
                Location = location,
                StartedAt = DateTime.UtcNow,
                LastActivityAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
            };
        }

        public void UpdateActivity()
        {
            if (HasEnded)
                throw new BusinessRuleValidationException("La sesión ya ha terminado");

            if (IsExpired)
                throw new BusinessRuleValidationException("La sesión ha expirado");

            LastActivityAt = DateTime.UtcNow;

            // Extender la expiración si la actividad está cerca del límite
            if ((ExpiresAt - DateTime.UtcNow).TotalMinutes < 10)
            {
                ExpiresAt = DateTime.UtcNow.AddMinutes(30);
            }
        }

        public void End()
        {
            if (HasEnded)
                throw new BusinessRuleValidationException("La sesión ya ha terminado");

            EndedAt = DateTime.UtcNow;
        }
    }
}
