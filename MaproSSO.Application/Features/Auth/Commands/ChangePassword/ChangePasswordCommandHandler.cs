using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Common.Models;

namespace MaproSSO.Application.Features.Auth.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;

        public ChangePasswordCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IPasswordHasher passwordHasher,
            ILogger<ChangePasswordCommandHandler> logger)
        {
            _context = context;
            _currentUser = currentUser;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.PasswordHistory)
                    .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, cancellationToken);

                if (user == null)
                {
                    return Result.Failure("Usuario no encontrado");
                }

                // Verificar contraseña actual
                if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.CurrentPassword))
                {
                    _logger.LogWarning("Intento de cambio de contraseña con contraseña actual incorrecta para usuario {UserId}",
                        _currentUser.UserId);
                    return Result.Failure("La contraseña actual es incorrecta");
                }

                // Validar fortaleza de la nueva contraseña
                if (!_passwordHasher.ValidatePasswordStrength(request.NewPassword, out var errors))
                {
                    return Result.Failure(errors);
                }

                // Hash de la nueva contraseña
                var newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);

                // Cambiar contraseña (esto validará que no esté en el historial)
                try
                {
                    user.ChangePassword(newPasswordHash);
                }
                catch (Exception ex)
                {
                    return Result.Failure(ex.Message);
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Contraseña cambiada exitosamente para usuario {UserId}", _currentUser.UserId);

                return Result.Success("Contraseña cambiada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar contraseña para usuario {UserId}", _currentUser.UserId);
                return Result.Failure("Error al cambiar la contraseña");
            }
        }
    }
}