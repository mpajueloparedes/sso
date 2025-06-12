using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Common.Models;

namespace MaproSSO.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;

        public ForgotPasswordCommandHandler(
            IApplicationDbContext context,
            ITokenService tokenService,
            IEmailService emailService,
            ILogger<ForgotPasswordCommandHandler> logger)
        {
            _context = context;
            _tokenService = tokenService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.NormalizedEmail == request.Email.ToUpperInvariant(),
                        cancellationToken);

                // Por seguridad, siempre retornamos éxito aunque el usuario no exista
                if (user == null)
                {
                    _logger.LogInformation("Solicitud de reseteo de contraseña para email no registrado: {Email}",
                        request.Email);
                    return Result.Success("Si el email está registrado, recibirá instrucciones para resetear su contraseña");
                }

                if (!user.IsActive)
                {
                    _logger.LogInformation("Solicitud de reseteo de contraseña para usuario inactivo: {Email}",
                        request.Email);
                    return Result.Success("Si el email está registrado, recibirá instrucciones para resetear su contraseña");
                }

                // Generar token de reseteo
                var resetToken = _tokenService.GeneratePasswordResetToken(user.Email.Value);

                // Enviar email
                await _emailService.SendTemplateAsync(
                    "PasswordReset",
                    user.Email.Value,
                    new Dictionary<string, object>
                    {
                        ["UserName"] = user.FirstName,
                        ["ResetToken"] = resetToken,
                        ["ResetUrl"] = $"https://maprosso.com/reset-password?token={resetToken}&email={user.Email.Value}"
                    });

                _logger.LogInformation("Email de reseteo de contraseña enviado a usuario {UserId}", user.Id);

                return Result.Success("Si el email está registrado, recibirá instrucciones para resetear su contraseña");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar solicitud de reseteo de contraseña");
                return Result.Failure("Error al procesar la solicitud");
            }
        }
    }
}
