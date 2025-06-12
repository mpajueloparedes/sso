using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Exceptions;
using MaproSSO.Domain.Entities.Announcements;

namespace MaproSSO.Application.Features.SSO.Announcements.Commands.UpdateAnnouncement
{
    public class UpdateAnnouncementCommandHandler : IRequestHandler<UpdateAnnouncementCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<UpdateAnnouncementCommandHandler> _logger;

        public UpdateAnnouncementCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            ILogger<UpdateAnnouncementCommandHandler> logger)
        {
            _context = context;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateAnnouncementCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var announcement = await _context.Announcements
                    .Include(a => a.Area)
                    .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

                if (announcement == null)
                {
                    throw new NotFoundException(nameof(Announcement), request.Id);
                }

                // Verificar permisos
                if (announcement.TenantId != _currentUser.TenantId)
                {
                    throw new ForbiddenAccessException("No tiene permisos para editar este anuncio");
                }

                // Solo el creador o admin puede editar
                if (announcement.CreatedBy != _currentUser.UserId &&
                    !_currentUser.HasRole("AdminSSO") &&
                    !_currentUser.HasRole("SuperAdmin"))
                {
                    throw new ForbiddenAccessException("Solo el creador o un administrador puede editar este anuncio");
                }

                announcement.Update(
                    request.Title,
                    request.Description,
                    request.Type,
                    request.Severity,
                    request.Location,
                    _currentUser.UserId.Value);

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Anuncio actualizado: {AnnouncementId} por usuario {UserId}",
                    request.Id, _currentUser.UserId);

                return Result.Success("Anuncio actualizado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar anuncio {AnnouncementId}", request.Id);
                throw;
            }
        }
    }
}