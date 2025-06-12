using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Common.Models;

namespace MaproSSO.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;

        public RefreshTokenCommandHandler(
            IApplicationDbContext context,
            ITokenService tokenService,
            ILogger<RefreshTokenCommandHandler> logger)
        {
            _context = context;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<Result<RefreshTokenResponse>> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Validar el access token (aunque esté expirado)
                var principal = await _tokenService.ValidateAccessToken(request.AccessToken);
                if (principal == null)
                {
                    return Result<RefreshTokenResponse>.Failure("Token de acceso inválido");
                }

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return Result<RefreshTokenResponse>.Failure("Token de acceso inválido");
                }

                // Buscar el refresh token
                var refreshToken = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt =>
                        rt.Token == request.RefreshToken &&
                        rt.UserId == userId,
                        cancellationToken);

                if (refreshToken == null)
                {
                    _logger.LogWarning("Refresh token no encontrado para usuario {UserId}", userId);
                    return Result<RefreshTokenResponse>.Failure("Refresh token inválido");
                }

                // Validar el refresh token
                if (refreshToken.IsRevoked)
                {
                    _logger.LogWarning("Intento de usar refresh token revocado para usuario {UserId}", userId);
                    return Result<RefreshTokenResponse>.Failure("Refresh token ha sido revocado");
                }

                if (refreshToken.IsExpired)
                {
                    _logger.LogWarning("Intento de usar refresh token expirado para usuario {UserId}", userId);
                    return Result<RefreshTokenResponse>.Failure("Refresh token ha expirado");
                }

                // Obtener el usuario con sus roles y permisos
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                            .ThenInclude(r => r.RolePermissions)
                                .ThenInclude(rp => rp.Permission)
                    .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

                if (user == null || !user.IsActive)
                {
                    return Result<RefreshTokenResponse>.Failure("Usuario no encontrado o inactivo");
                }

                // Revocar el token actual
                refreshToken.Revoke("Token renovado", userId);

                // Generar nuevos tokens
                var newAccessToken = await _tokenService.GenerateAccessToken(user);
                var newRefreshToken = await _tokenService.GenerateRefreshToken();

                // Crear nuevo refresh token
                var newRefreshTokenEntity = user.GenerateRefreshToken(
                    newRefreshToken,
                    refreshToken.DeviceInfo,
                    request.IpAddress);

                // Actualizar la sesión si existe
                var session = await _context.UserSessions
                    .FirstOrDefaultAsync(s =>
                        s.UserId == userId &&
                        s.IsActive &&
                        !s.HasEnded,
                        cancellationToken);

                session?.UpdateActivity();

                await _context.SaveChangesAsync(cancellationToken);

                var response = new RefreshTokenResponse
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresIn = 900, // 15 minutos
                    TokenType = "Bearer"
                };

                _logger.LogInformation("Tokens renovados exitosamente para usuario {UserId}", userId);

                return Result<RefreshTokenResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al renovar tokens");
                return Result<RefreshTokenResponse>.Failure("Error al renovar tokens");
            }
        }
    }
}