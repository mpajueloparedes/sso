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
using MaproSSO.Application.Common.Services;
using MaproSSO.Domain.Entities.Announcements;

namespace MaproSSO.Application.Features.SSO.Announcements.Commands.AddCorrectiveAction
{
    public class AddCorrectiveActionCommandHandler : IRequestHandler<AddCorrectiveActionCommand, Result<CorrectiveActionDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly ILogger<AddCorrectiveActionCommandHandler> _logger;

        public AddCorrectiveActionCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IMapper mapper,
            INotificationService notificationService,
            ILogger<AddCorrectiveActionCommandHandler> logger)
        {
            _context = context;
            _currentUser = currentUser;
            _mapper = mapper;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<Result<CorrectiveActionDto>> Handle(
            AddCorrectiveActionCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var announcement = await _context.Announcements
                    .Include(a => a.CorrectiveActions)
                    .FirstOrDefaultAsync(a => a.Id == request.AnnouncementId, cancellationToken);

                if (announcement == null)
                {
                    throw new NotFoundException(nameof(Announcement), request.AnnouncementId);
                }

                // Verificar permisos
                if (announcement.TenantId != _currentUser.TenantId)
                {
                    throw new ForbiddenAccessException("No tiene permisos para agregar acciones a este anuncio");
                }

                // Verificar que el usuario responsable existe y pertenece al tenant
                var responsibleUser = await _context.Users
                    .FirstOrDefaultAsync(u =>
                        u.Id == request.ResponsibleUserId &&
                        u.TenantId == _currentUser.TenantId &&
                        u.IsActive,
                        cancellationToken);

                if (responsibleUser == null)
                {
                    return Result<CorrectiveActionDto>.Failure("Usuario responsable no válido");
                }

                // Agregar acción correctiva
                var action = announcement.AddCorrectiveAction(
                    request.Description,
                    request.ResponsibleUserId,
                    request.DueDate,
                    _currentUser.UserId.Value);

                await _context.SaveChangesAsync(cancellationToken);

                // Notificar al responsable
                await _notificationService.SendNotificationAsync(new NotificationRequest
                {
                    UserId = request.ResponsibleUserId,
                    Subject = "Nueva acción correctiva asignada",
                    Message = $"Se le ha asignado una acción correctiva para el anuncio: {announcement.Title}",
                    Type = NotificationType.Info,
                    Channel = NotificationChannel.InApp | NotificationChannel.Email,
                    Data = new Dictionary<string, object>
                    {
                        ["AnnouncementId"] = announcement.Id,
                        ["ActionId"] = action.Id,
                        ["DueDate"] = request.DueDate
                    }
                });

                var dto = _mapper.Map<CorrectiveActionDto>(action);
                dto.ResponsibleUserName = responsibleUser.FullName;

                _logger.LogInformation(
                    "Acción correctiva agregada: {ActionId} al anuncio {AnnouncementId}",
                    action.Id, announcement.Id);

                return Result<CorrectiveActionDto>.Success(dto, "Acción correctiva agregada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar acción correctiva");
                throw;
            }
        }
    }
}