using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Common.Models;

namespace MaproSSO.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<ResetPasswordCommandHandler> _logger;

        public ResetPasswordCommandHandler(
            IApplicationDbContext context,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            ILogger<ResetPasswordCommandHandler> logger)
        {
            _context = context;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validar token
                if (!_tokenService.ValidatePasswordResetToken(request.Token, request.Email))
                {
                    return Result.Failure("Token inválido o expirado");
                }

                var user = await _context.Users
                    .Include(u => u.PasswordHistory)
                    .FirstOrDefaultAsync(u => u.NormalizedEmail == request.Email.ToUpperInvariant(),
                        cancellationToken);

                if (user == null)
                {
                    return Result.Failure("Usuario no encontrado");
                }

                // Validar fortaleza de la nueva contraseña
                if (!_passwordHasher.ValidatePasswordStrength(request.NewPassword, out var errors))
                {
                    return Result.Failure(errors);
                }

                // Hash de la nueva contraseña
                var newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);

                // Cambiar contraseña
                try
                {
                    user.ChangePassword(newPasswordHash);

                    // Revocar todos los tokens por seguridad
                    user.RevokeAllRefreshTokens("Contraseña reseteada", user.Id);
                }
                catch (Exception ex)
                {
                    return Result.Failure(ex.Message);
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Contraseña reseteada exitosamente para usuario {UserId}", user.Id);

                return Result.Success("Contraseña reseteada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al resetear contraseña");
                return Result.Failure("Error al resetear la contraseña");
            }
        }
    }
}