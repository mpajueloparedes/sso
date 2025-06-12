using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Exceptions;
using MaproSSO.Domain.Entities.Security;

namespace MaproSSO.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ISubscriptionValidationService _subscriptionService;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            IApplicationDbContext context,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            ISubscriptionValidationService subscriptionService,
            ILogger<LoginCommandHandler> logger)
        {
            _context = context;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public async Task<Result<LoginResponse>> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Buscar usuario por username o email
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                            .ThenInclude(r => r.RolePermissions)
                                .ThenInclude(rp => rp.Permission)
                    .FirstOrDefaultAsync(u =>
                        u.Username == request.Username.ToLowerInvariant() ||
                        u.NormalizedEmail == request.Username.ToUpperInvariant(),
                        cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("Intento de login fallido para usuario: {Username}", request.Username);
                    return Result<LoginResponse>.Failure("Usuario o contraseña incorrectos");
                }

                // Verificar si el usuario está bloqueado
                if (user.IsLockedOut)
                {
                    _logger.LogWarning("Intento de login a cuenta bloqueada: {Username}", request.Username);
                    return Result<LoginResponse>.Failure($"Cuenta bloqueada hasta {user.LockoutEnd:dd/MM/yyyy HH:mm}");
                }

                // Verificar contraseña
                if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
                {
                    user.RecordFailedLogin();
                    await _context.SaveChangesAsync(cancellationToken);

                    _logger.LogWarning("Contraseña incorrecta para usuario: {Username}", request.Username);

                    if (user.IsLockedOut)
                    {
                        return Result<LoginResponse>.Failure($"Cuenta bloqueada por múltiples intentos fallidos hasta {user.LockoutEnd:dd/MM/yyyy HH:mm}");
                    }

                    return Result<LoginResponse>.Failure("Usuario o contraseña incorrectos");
                }

                // Verificar si el usuario está activo
                if (!user.IsActive)
                {
                    _logger.LogWarning("Intento de login a cuenta inactiva: {Username}", request.Username);
                    return Result<LoginResponse>.Failure("La cuenta de usuario está inactiva");
                }

                // Verificar si el tenant está activo
                var tenant = await _context.Tenants
                    .FirstOrDefaultAsync(t => t.Id == user.TenantId, cancellationToken);

                if (tenant == null || !tenant.IsActive)
                {
                    _logger.LogWarning("Intento de login con tenant inactivo: {TenantId}", user.TenantId);
                    return Result<LoginResponse>.Failure("La empresa está inactiva");
                }

                // Verificar suscripción del tenant (excepto SuperAdmin)
                if (!user.HasRole("SuperAdmin"))
                {
                    var subscriptionValid = await _subscriptionService.ValidateSubscription(user.TenantId);
                    if (!subscriptionValid)
                    {
                        _logger.LogWarning("Intento de login con suscripción expirada: {TenantId}", user.TenantId);
                        return Result<LoginResponse>.Failure("La suscripción de su empresa ha expirado. Contacte al administrador.");
                    }

                    var subscriptionStatus = await _subscriptionService.GetSubscriptionStatus(user.TenantId);
                    if (subscriptionStatus == Domain.Enums.SubscriptionStatus.Suspended)
                    {
                        return Result<LoginResponse>.Failure("La suscripción está suspendida. Su acceso es de solo lectura.");
                    }
                }

                // Verificar si debe cambiar contraseña
                if (user.ShouldChangePassword() && !user.MustChangePassword)
                {
                    user.ForcePasswordChange();
                    await _context.SaveChangesAsync(cancellationToken);
                }

                // Generar tokens
                var accessToken = await _tokenService.GenerateAccessToken(user);
                var refreshToken = await _tokenService.GenerateRefreshToken();

                // Guardar refresh token
                var refreshTokenEntity = user.GenerateRefreshToken(
                    refreshToken,
                    request.DeviceInfo,
                    request.IpAddress,
                    request.RememberMe ? 30 : 7); // 30 días si RememberMe, 7 días si no

                // Registrar login exitoso
                user.RecordSuccessfulLogin(request.IpAddress);

                // Crear sesión
                var deviceInfo = ParseDeviceInfo(request.DeviceInfo);
                var session = user.CreateSession(
                    Guid.NewGuid().ToString(),
                    deviceInfo.DeviceType,
                    deviceInfo.DeviceName,
                    deviceInfo.Browser,
                    deviceInfo.OperatingSystem,
                    request.IpAddress);

                _context.UserSessions.Add(session);
                await _context.SaveChangesAsync(cancellationToken);

                // Preparar respuesta
                var roles = user.UserRoles
                    .Select(ur => ur.Role)
                    .Where(r => r != null)
                    .Select(r => r.RoleName)
                    .ToList();

                var permissions = user.UserRoles
                    .SelectMany(ur => ur.Role?.RolePermissions ?? Enumerable.Empty<RolePermission>())
                    .Where(rp => rp.Permission != null)
                    .Select(rp => rp.Permission.PermissionCode)
                    .Distinct()
                    .ToList();

                var response = new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresIn = 900, // 15 minutos
                    TokenType = "Bearer",
                    SessionId = session.Id.ToString(),
                    User = new UserDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email.Value,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        FullName = user.FullName,
                        PhotoUrl = user.PhotoUrl,
                        TenantId = user.TenantId,
                        TenantName = tenant.CompanyName,
                        TimeZone = tenant.TimeZone,
                        Culture = tenant.Culture,
                        Roles = roles,
                        Permissions = permissions
                    },
                    RequiresTwoFactor = user.TwoFactorEnabled,
                    MustChangePassword = user.MustChangePassword
                };

                _logger.LogInformation("Login exitoso para usuario: {Username} desde IP: {IpAddress}",
                    user.Username, request.IpAddress);

                return Result<LoginResponse>.Success(response, "Login exitoso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el login para usuario: {Username}", request.Username);
                return Result<LoginResponse>.Failure("Error al procesar el login. Intente nuevamente.");
            }
        }

        private DeviceInfo ParseDeviceInfo(string deviceInfoString)
        {
            // Implementar parsing del user agent o información del dispositivo
            // Por ahora retornamos valores por defecto
            return new DeviceInfo
            {
                DeviceType = "Web",
                DeviceName = "Browser",
                Browser = "Chrome",
                OperatingSystem = "Windows"
            };
        }

        private class DeviceInfo
        {
            public string DeviceType { get; set; }
            public string DeviceName { get; set; }
            public string Browser { get; set; }
            public string OperatingSystem { get; set; }
        }
    }
}