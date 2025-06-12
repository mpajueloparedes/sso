//using AutoMapper;
//using MaproSSO.Application.Common.Interfaces;
//using MaproSSO.Application.Common.Models;
//using MaproSSO.Application.Common.Services;
//using MaproSSO.Domain.Entities.SSO;
//using MediatR;
//using Microsoft.Extensions.Logging;

//uusing System;
//using System.Threading;
//using System.Threading.Tasks;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using AutoMapper;
//using MaproSSO.Application.Common.Interfaces;
//using MaproSSO.Application.Common.Models;
//using MaproSSO.Application.Common.Exceptions;
//using MaproSSO.Domain.Entities.SSO;

//namespace MaproSSO.Application.Features.SSO.Announcements.Commands.CreateAnnouncement
//{
//    public class CreateAnnouncementCommandHandler : IRequestHandler<CreateAnnouncementCommand, Result<AnnouncementDto>>
//    {
//        private readonly IApplicationDbContext _context;
//        private readonly ICurrentUserService _currentUser;
//        private readonly IMapper _mapper;
//        private readonly ISubscriptionValidationService _subscriptionService;
//        private readonly INotificationService _notificationService;
//        private readonly ILogger<CreateAnnouncementCommandHandler> _logger;

//        public CreateAnnouncementCommandHandler(
//            IApplicationDbContext context,
//            ICurrentUserService currentUser,
//            IMapper mapper,
//            ISubscriptionValidationService subscriptionService,
//            INotificationService notificationService,
//            ILogger<CreateAnnouncementCommandHandler> logger)
//        {
//            _context = context;
//            _currentUser = currentUser;
//            _mapper = mapper;
//            _subscriptionService = subscriptionService;
//            _notificationService = notificationService;
//            _logger = logger;
//        }

//        public async Task<Result<AnnouncementDto>> Handle(
//            CreateAnnouncementCommand request,
//            CancellationToken cancellationToken)
//        {
//            try
//            {
//                // Verificar acceso al módulo
//                var hasAccess = await _subscriptionService.ValidateFeatureAccess(
//                    _currentUser.TenantId.Value, "module_announcements");

//                if (!hasAccess)
//                {
//                    return Result<AnnouncementDto>.Failure("No tiene acceso al módulo de anuncios");
//                }

//                // Verificar que el área pertenece al tenant y el usuario tiene acceso
//                var area = await _context.Areas
//                    .Include(a => a.AreaUsers)
//                    .FirstOrDefaultAsync(a =>
//                        a.Id == request.AreaId &&
//                        a.TenantId == _currentUser.TenantId,
//                        cancellationToken);

//                if (area == null)
//                {
//                    return Result<AnnouncementDto>.Failure("Área no encontrada");
//                }

//                // Verificar que el usuario pertenece al área o es admin
//                if (!_currentUser.HasRole("AdminSSO") && !_currentUser.HasRole("SuperAdmin"))
//                {
//                    var userBelongsToArea = area.HasUser(_currentUser.UserId.Value);
//                    if (!userBelongsToArea)
//                    {
//                        return Result<AnnouncementDto>.Failure("No tiene permisos para crear anuncios en esta área");
//                    }
//                }

//                // Crear anuncio
//                var announcement = Announcement.Create(
//                    _currentUser.TenantId.Value,
//                    request.AreaId,
//                    request.Title,
//                    request.Description,
//                    request.Type,
//                    request.Severity,
//                    request.Location,
//                    _currentUser.UserId.Value);

//                // Agregar imágenes
//                foreach (var image in request.Images)
//                {
//                    announcement.AddImage(image.ImageUrl, image.Description, _currentUser.UserId);
//                }

//                _context.Announcements.Add(announcement);

//                // Actualizar uso de almacenamiento
//                if (request.Images.Count > 0)
//                {
//                    await _subscriptionService.IncrementFeatureUsage(
//                        _currentUser.TenantId.Value,
//                        "storage_used_mb",
//                        request.Images.Count * 2); // Aproximado 2MB por imagen
//                }

//                await _context.SaveChangesAsync(cancellationToken);

//                // Enviar notificaciones según la severidad
//                if (request.Severity == Severity.High || request.Severity == Severity.Critical)
//                {
//                    // Notificar a los líderes del área
//                    var leaders = area.AreaUsers
//                        .Where(au => au.Role == "Leader")
//                        .Select(au => au.UserId)
//                        .ToList();

//                    foreach (var leaderId in leaders)
//                    {
//                        await _notificationService.SendNotificationAsync(new NotificationRequest
//                        {
//                            UserId = leaderId,
//                            Subject = $"Anuncio {request.Severity}: {request.Title}",
//                            Message = $"Se ha creado un anuncio de severidad {request.Severity} en el área {area.AreaName}",
//                            Type = NotificationType.Warning,
//                            Channel = NotificationChannel.InApp | NotificationChannel.Email,
//                            Data = new Dictionary<string, object>
//                            {
//                                ["AnnouncementId"] = announcement.Id,
//                                ["AreaId"] = area.Id,
//                                ["Severity"] = request.Severity.ToString()
//                            }
//                        });
//                    }
//                }

//                var dto = _mapper.Map<AnnouncementDto>(announcement);

//                _logger.LogInformation(
//                    "Anuncio creado: {AnnouncementId} en área {AreaId} por usuario {UserId}",
//                    announcement.Id, request.AreaId, _currentUser.UserId);

//                return Result<AnnouncementDto>.Success(dto, "Anuncio creado exitosamente");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex