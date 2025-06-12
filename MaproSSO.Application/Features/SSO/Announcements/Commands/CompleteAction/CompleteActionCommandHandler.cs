using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Exceptions;

namespace MaproSSO.Application.Features.SSO.Announcements.Commands.CompleteAction
{
    public class CompleteActionCommandHandler : IRequestHandler<CompleteActionCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<CompleteActionCommandHandler> _logger;

        public CompleteActionCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            ILogger<CompleteActionCommandHandler> logger)
        {
            _context = context;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<Result> Handle(CompleteActionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var announcement = await _context.Announcements
                    .Include(a => a.CorrectiveActions)
                        .ThenInclude(ca => ca.Evidences)
                    .FirstOrDefaultAsync(a => a.CorrectiveActions.Any(ca => ca.Id == request.ActionId),
                        cancellationToken);

                if (announcement == null)
                {
                    throw new NotFoundException("Acción correctiva", request.ActionId);
                }

                // Verificar permisos
                if (announcement.TenantId != _currentUser.TenantId)
                {
                    throw new ForbiddenAccessException("No tiene permisos para completar esta acción");
                }

                announcement.CompleteCorrectiveAction(request.ActionId, _currentUser.UserId.Value);

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Acción correctiva completada: {ActionId} del anuncio {AnnouncementId}",
                    request.ActionId, announcement.Id);

                var message = announcement.Status == Domain.Enums.AnnouncementStatus.Completed
                    ? "Acción completada. El anuncio ha sido cerrado."
                    : "Acción completada exitosamente.";

                return Result.Success(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al completar acción correctiva");
                throw;
            }
        }
    }
}
