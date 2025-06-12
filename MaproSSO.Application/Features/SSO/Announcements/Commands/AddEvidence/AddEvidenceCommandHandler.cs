using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Exceptions;
using MaproSSO.Domain.Entities.SSO;

namespace MaproSSO.Application.Features.SSO.Announcements.Commands.AddEvidence
{
    public class AddEvidenceCommandHandler : IRequestHandler<AddEvidenceCommand, Result<EvidenceDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IFileStorageService _fileStorage;
        private readonly IMapper _mapper;
        private readonly ILogger<AddEvidenceCommandHandler> _logger;

        public AddEvidenceCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IFileStorageService fileStorage,
            IMapper mapper,
            ILogger<AddEvidenceCommandHandler> logger)
        {
            _context = context;
            _currentUser = currentUser;
            _fileStorage = fileStorage;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<EvidenceDto>> Handle(
            AddEvidenceCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var action = await _context.CorrectiveActions
                    .Include(ca => ca.Announcement)
                    .FirstOrDefaultAsync(ca => ca.Id == request.ActionId, cancellationToken);

                if (action == null)
                {
                    throw new NotFoundException(nameof(CorrectiveAction), request.ActionId);
                }

                // Verificar permisos - solo el responsable o admin puede agregar evidencias
                if (action.ResponsibleUserId != _currentUser.UserId &&
                    !_currentUser.HasRole("AdminSSO") &&
                    !_currentUser.HasRole("SuperAdmin"))
                {
                    throw new ForbiddenAccessException("Solo el responsable puede agregar evidencias");
                }

                // Subir archivo
                var containerName = $"tenant-{_currentUser.TenantId}/announcements";
                var fileName = $"evidence_{DateTime.UtcNow.Ticks}_{request.FileName}";
                var fileUrl = await _fileStorage.UploadAsync(request.FileStream, fileName, containerName);

                // Agregar evidencia
                action.AddEvidence(request.Description, fileUrl, _currentUser.UserId.Value);

                await _context.SaveChangesAsync(cancellationToken);

                var evidence = action.Evidences.Last();
                var dto = _mapper.Map<EvidenceDto>(evidence);

                _logger.LogInformation(
                    "Evidencia agregada: {EvidenceId} a la acción {ActionId}",
                    evidence.Id, action.Id);

                return Result<EvidenceDto>.Success(dto, "Evidencia agregada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar evidencia");
                throw;
            }
        }
    }
}