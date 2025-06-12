using MaproSSO.Domain.Common;
using MaproSSO.Domain.Enums;
using MaproSSO.Domain.Exceptions;
using MaproSSO.Domain.ValueObjects;
using static MaproSSO.Domain.Entities.SSO.AuditCriteria;

namespace MaproSSO.Domain.Entities.Security
{
    public class User : BaseAuditableEntity, IAggregateRoot
    {
        private readonly List<UserRole> _userRoles = new();
        private readonly List<RefreshToken> _refreshTokens = new();
        private readonly List<PasswordHistory> _passwordHistory = new();
        private readonly List<UserSession> _sessions = new();

        public Guid TenantId { get; private set; }
        public string Username { get; private set; }
        public Email Email { get; private set; }
        public string NormalizedEmail { get; private set; }
        public bool EmailConfirmed { get; private set; }
        public string PasswordHash { get; private set; }
        public string SecurityStamp { get; private set; }
        public string PhoneNumber { get; private set; }
        public bool PhoneNumberConfirmed { get; private set; }
        public bool TwoFactorEnabled { get; private set; }
        public TwoFactorMethod? TwoFactorMethod { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string EmployeeCode { get; private set; }
        public string Position { get; private set; }
        public string Department { get; private set; }
        public string PhotoUrl { get; private set; }
        public DateTime? LockoutEnd { get; private set; }
        public bool LockoutEnabled { get; private set; }
        public int AccessFailedCount { get; private set; }
        public DateTime? PasswordChangedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public string LastLoginIP { get; private set; }
        public bool IsActive { get; private set; }
        public bool MustChangePassword { get; private set; }

        public string FullName => $"{FirstName} {LastName}";
        public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;

        public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
        public IReadOnlyCollection<PasswordHistory> PasswordHistory => _passwordHistory.AsReadOnly();
        public IReadOnlyCollection<UserSession> Sessions => _sessions.AsReadOnly();

        private User() { }

        public static User Create(
            Guid tenantId,
            string username,
            Email email,
            string firstName,
            string lastName,
            string passwordHash,
            Guid createdBy)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new DomainException("El nombre de usuario es requerido");

            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("El nombre es requerido");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("El apellido es requerido");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("La contraseña es requerida");

            var user = new User
            {
                TenantId = tenantId,
                Username = username.ToLowerInvariant(),
                Email = email ?? throw new ArgumentNullException(nameof(email)),
                NormalizedEmail = email.Value.ToUpperInvariant(),
                EmailConfirmed = false,
                PasswordHash = passwordHash,
                SecurityStamp = Guid.NewGuid().ToString(),
                FirstName = firstName,
                LastName = lastName,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                IsActive = true,
                MustChangePassword = false,
                PasswordChangedAt = DateTime.UtcNow
            };

            user.SetCreatedInfo(createdBy);
            user.AddPasswordToHistory(passwordHash);

            return user;
        }

        public void UpdateProfile(
            string firstName,
            string lastName,
            string employeeCode,
            string position,
            string department,
            string phoneNumber,
            Guid updatedBy)
        {
            FirstName = firstName;
            LastName = lastName;
            EmployeeCode = employeeCode;
            Position = position;
            Department = department;
            PhoneNumber = phoneNumber;

            SetUpdatedInfo(updatedBy);
        }

        public void UpdatePhoto(string photoUrl, Guid updatedBy)
        {
            PhotoUrl = photoUrl;
            SetUpdatedInfo(updatedBy);
        }

        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new DomainException("La nueva contraseña es requerida");

            // Verificar que la nueva contraseña no esté en el historial
            if (_passwordHistory.Take(5).Any(p => p.PasswordHash == newPasswordHash))
                throw new BusinessRuleValidationException("La contraseña no puede ser igual a las últimas 5 contraseñas utilizadas");

            PasswordHash = newPasswordHash;
            PasswordChangedAt = DateTime.UtcNow;
            MustChangePassword = false;
            SecurityStamp = Guid.NewGuid().ToString();

            AddPasswordToHistory(newPasswordHash);
        }

        private void AddPasswordToHistory(string passwordHash)
        {
            _passwordHistory.Add(new PasswordHistory(Id, passwordHash));

            // Mantener solo las últimas 5 contraseñas
            if (_passwordHistory.Count > 5)
            {
                var oldestPasswords = _passwordHistory
                    .OrderBy(p => p.CreatedAt)
                    .Take(_passwordHistory.Count - 5)
                    .ToList();

                foreach (var old in oldestPasswords)
                {
                    _passwordHistory.Remove(old);
                }
            }
        }

        public void ForcePasswordChange()
        {
            MustChangePassword = true;
            SecurityStamp = Guid.NewGuid().ToString();
        }

        public void ConfirmEmail()
        {
            EmailConfirmed = true;
        }

        public void ChangeEmail(Email newEmail, Guid updatedBy)
        {
            if (newEmail == null)
                throw new ArgumentNullException(nameof(newEmail));

            Email = newEmail;
            NormalizedEmail = newEmail.Value.ToUpperInvariant();
            EmailConfirmed = false;
            SecurityStamp = Guid.NewGuid().ToString();

            SetUpdatedInfo(updatedBy);
        }

        public void ConfirmPhoneNumber()
        {
            if (string.IsNullOrWhiteSpace(PhoneNumber))
                throw new BusinessRuleValidationException("No hay número de teléfono para confirmar");

            PhoneNumberConfirmed = true;
        }

        public void RecordSuccessfulLogin(string ipAddress)
        {
            LastLoginAt = DateTime.UtcNow;
            LastLoginIP = ipAddress;
            AccessFailedCount = 0;

            if (LockoutEnd.HasValue)
                LockoutEnd = null;
        }

        public void RecordFailedLogin()
        {
            AccessFailedCount++;

            if (AccessFailedCount >= 5 && LockoutEnabled)
            {
                LockoutEnd = DateTime.UtcNow.AddMinutes(30);
            }
        }

        public void ResetAccessFailedCount()
        {
            AccessFailedCount = 0;
            LockoutEnd = null;
        }

        public void LockOut(DateTime until)
        {
            if (!LockoutEnabled)
                throw new BusinessRuleValidationException("El bloqueo no está habilitado para este usuario");

            LockoutEnd = until;
        }

        public void Unlock()
        {
            LockoutEnd = null;
            AccessFailedCount = 0;
        }

        public void EnableTwoFactor(TwoFactorMethod method)
        {
            TwoFactorEnabled = true;
            TwoFactorMethod = method;
            SecurityStamp = Guid.NewGuid().ToString();
        }

        public void DisableTwoFactor()
        {
            TwoFactorEnabled = false;
            TwoFactorMethod = null;
            SecurityStamp = Guid.NewGuid().ToString();
        }

        public void AssignRole(Guid roleId, Guid assignedBy)
        {
            if (_userRoles.Any(ur => ur.RoleId == roleId))
                throw new BusinessRuleValidationException("El usuario ya tiene este rol asignado");

            _userRoles.Add(new UserRole(Id, roleId, assignedBy));
        }

        public void RemoveRole(Guid roleId)
        {
            var userRole = _userRoles.FirstOrDefault(ur => ur.RoleId == roleId);
            if (userRole == null)
                throw new BusinessRuleValidationException("El usuario no tiene este rol asignado");

            _userRoles.Remove(userRole);
        }

        public bool HasRole(string roleName)
        {
            return _userRoles.Any(ur => ur.Role?.NormalizedRoleName == roleName.ToUpperInvariant());
        }

        public RefreshToken GenerateRefreshToken(string token, string deviceInfo, string ipAddress, int expirationDays = 7)
        {
            var refreshToken = RefreshToken.Create(Id, token, deviceInfo, ipAddress, expirationDays);
            _refreshTokens.Add(refreshToken);

            // Limpiar tokens antiguos
            var expiredTokens = _refreshTokens.Where(rt => rt.IsExpired && !rt.IsRevoked).ToList();
            foreach (var expired in expiredTokens)
            {
                _refreshTokens.Remove(expired);
            }

            return refreshToken;
        }

        public void RevokeRefreshToken(string token, string reason, Guid revokedBy)
        {
            var refreshToken = _refreshTokens.FirstOrDefault(rt => rt.Token == token && !rt.IsRevoked);
            if (refreshToken == null)
                throw new BusinessRuleValidationException("Token no válido");

            refreshToken.Revoke(reason, revokedBy);
        }

        public void RevokeAllRefreshTokens(string reason, Guid revokedBy)
        {
            foreach (var token in _refreshTokens.Where(rt => !rt.IsRevoked))
            {
                token.Revoke(reason, revokedBy);
            }
        }

        public UserSession CreateSession(string sessionToken, string deviceType, string deviceName,
            string browser, string operatingSystem, string ipAddress, string location = null)
        {
            // Verificar sesiones concurrentes
            var activeSessions = _sessions.Where(s => !s.HasEnded && !s.IsExpired).ToList();
            if (activeSessions.Count >= 3)
            {
                // Terminar la sesión más antigua
                var oldestSession = activeSessions.OrderBy(s => s.StartedAt).First();
                oldestSession.End();
            }

            var session = UserSession.Create(Id, sessionToken, deviceType, deviceName,
                browser, operatingSystem, ipAddress, location);

            _sessions.Add(session);

            return session;
        }

        public void EndSession(string sessionToken)
        {
            var session = _sessions.FirstOrDefault(s => s.SessionToken == sessionToken && !s.HasEnded);
            if (session == null)
                throw new BusinessRuleValidationException("Sesión no encontrada");

            session.End();
        }

        public void EndAllSessions()
        {
            foreach (var session in _sessions.Where(s => !s.HasEnded))
            {
                session.End();
            }
        }

        public void UpdateSessionActivity(string sessionToken)
        {
            var session = _sessions.FirstOrDefault(s => s.SessionToken == sessionToken && !s.HasEnded);
            session?.UpdateActivity();
        }

        public void Deactivate(Guid userId)
        {
            if (!IsActive)
                throw new BusinessRuleValidationException("El usuario ya está inactivo");

            IsActive = false;
            SetUpdatedInfo(userId);

            // Revocar todos los tokens y sesiones
            RevokeAllRefreshTokens("Usuario desactivado", userId);
            EndAllSessions();
        }

        public void Activate(Guid userId)
        {
            if (IsActive)
                throw new BusinessRuleValidationException("El usuario ya está activo");

            IsActive = true;
            SetUpdatedInfo(userId);
        }

        public bool ShouldChangePassword()
        {
            if (MustChangePassword) return true;

            // Forzar cambio de contraseña cada 90 días
            if (PasswordChangedAt.HasValue)
            {
                var daysSinceChange = (DateTime.UtcNow - PasswordChangedAt.Value).TotalDays;
                return daysSinceChange > 90;
            }

            return false;
        }
    }
}