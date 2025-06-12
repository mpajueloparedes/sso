using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Common.Models;

namespace MaproSSO.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<LogoutCommandHandler> _logger;

        public LogoutCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            ILogger<LogoutCommandHandler> logger)
        {
            _context = context;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, cancellationToken);

                if (user == null)
                {
                    return Result.Failure("Usuario no encontrado");
                }

                if (request.LogoutFromAllDevices)
                {
                    // Revocar todos los refresh tokens
                    user.RevokeAllRefreshTokens("Logout desde todos los dispositivos", _currentUser.UserId.Value);

                    // Terminar todas las sesiones
                    user.EndAllSessions();

                    _logger.LogInformation("Usuario {UserId} cerró sesión en todos los dispositivos", _currentUser.UserId);
                }
                else
                {
                    // Revocar el refresh token específico
                    if (!string.IsNullOrEmpty(request.RefreshToken))
                    {
                        user.RevokeRefreshToken(request.RefreshToken, "Logout", _currentUser.UserId.Value);
                    }

                    // Terminar la sesión específica
                    if (!string.IsNullOrEmpty(request.SessionId) && Guid.TryParse(request.SessionId, out var sessionId))
                    {
                        var session = await _context.UserSessions
                            .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == user.Id, cancellationToken);

                        session?.End();
                    }

                    _logger.LogInformation("Usuario {UserId} cerró sesión", _currentUser.UserId);
                }

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success("Sesión cerrada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cerrar sesión para usuario {UserId}", _currentUser.UserId);
                return Result.Failure("Error al cerrar sesión");
            }
        }
    }
}