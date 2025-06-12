using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Exceptions;
using MaproSSO.Domain.Entities.SSO;

namespace MaproSSO.Application.Features.SSO.Inspections.Commands.StartInspection
{
    public class StartInspectionCommandHandler : IRequestHandler<StartInspectionCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IFileStorageService _fileStorage;
        private readonly ILogger<StartInspectionCommandHandler> _logger;

        public StartInspectionCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IFileStorageService fileStorage,
            ILogger<StartInspectionCommandHandler> logger)
        {
            _context = context;
            _currentUser = currentUser;
            _fileStorage = fileStorage;
            _logger = logger;
        }

        public async Task<r> Handle(StartInspectionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var inspection = await _context.Inspections
                    .FirstOrDefaultAsync(i => i.Id == request.InspectionId, cancellationToken);

                if (inspection == null)
                {
                    throw new NotFoundException(nameof(Inspection), request.InspectionId);
                }

                // Verificar permisos - solo el inspector asignado puede iniciar
                if (inspection.InspectorUserId != _currentUser.UserId &&
                    !_currentUser.HasRole("AdminSSO") &&
                    !_currentUser.HasRole("SuperAdmin"))
                {
                    throw new ForbiddenAccessException("Solo el inspector asignado puede iniciar la inspección");
                }

                // Subir documento
                var containerName = $"tenant-{_currentUser.TenantId}/inspections";
                var fileName = $"inspection_{inspection.InspectionCode}_{DateTime.UtcNow.Ticks}_{request.FileName}";
                var documentUrl = await _fileStorage.UploadAsync(request.DocumentStream, fileName, containerName);

                // Iniciar inspección
                inspection.Start(documentUrl, _currentUser.UserId.Value);

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Inspección iniciada: {InspectionId} por usuario {UserId}",
                    request.InspectionId, _currentUser.UserId);

                return Result.Success("Inspección iniciada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar inspección");
                throw;
            }
        }
    }
}