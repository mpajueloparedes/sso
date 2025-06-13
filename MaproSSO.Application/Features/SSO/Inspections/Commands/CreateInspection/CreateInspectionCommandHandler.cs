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
using MaproSSO.Application.Common.Services;
using MaproSSO.Application.Features.Inspections.DTOs;

namespace MaproSSO.Application.Features.SSO.Inspections.Commands.CreateInspection
{
    public class CreateInspectionCommandHandler : IRequestHandler<CreateInspectionCommand, Result<InspectionDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;
        private readonly ISubscriptionValidationService _subscriptionService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<CreateInspectionCommandHandler> _logger;

        public CreateInspectionCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IMapper mapper,
            ISubscriptionValidationService subscriptionService,
            INotificationService notificationService,
            ILogger<CreateInspectionCommandHandler> logger)
        {
            _context = context;
            _currentUser = currentUser;
            _mapper = mapper;
            _subscriptionService = subscriptionService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<Result<InspectionDto>> Handle(
            CreateInspectionCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Verificar acceso al módulo
                var hasAccess = await _subscriptionService.ValidateFeatureAccess(
                    _currentUser.TenantId.Value, "module_inspections");

                if (!hasAccess)
                {
                    return Result<InspectionDto>.Failure("No tiene acceso al módulo de inspecciones");
                }

                // Verificar que el programa existe y pertenece al tenant
                var program = await _context.InspectionPrograms
                    .FirstOrDefaultAsync(p =>
                        p.Id == request.ProgramId &&
                        p.TenantId == _currentUser.TenantId &&
                        p.IsActive,
                        cancellationToken);

                if (program == null)
                {
                    return Result<InspectionDto>.Failure("Programa de inspección no encontrado");
                }

                // Verificar que el área existe y pertenece al tenant
                var area = await _context.Areas
                    .FirstOrDefaultAsync(a =>
                        a.Id == request.AreaId &&
                        a.TenantId == _currentUser.TenantId &&
                        a.IsActive,
                        cancellationToken);

                if (area == null)
                {
                    return Result<InspectionDto>.Failure("Área no encontrada");
                }

                // Verificar que el inspector existe y pertenece al tenant
                var inspector = await _context.Users
                    .FirstOrDefaultAsync(u =>
                        u.Id == request.InspectorUserId &&
                        u.TenantId == _currentUser.TenantId &&
                        u.IsActive,
                        cancellationToken);

                if (inspector == null)
                {
                    return Result<InspectionDto>.Failure("Inspector no válido");
                }

                // Generar código de inspección
                var inspectionCode = await GenerateInspectionCode(cancellationToken);

                // Crear inspección
                var inspection = Inspection.Create(
                    _currentUser.TenantId.Value,
                    request.ProgramId,
                    request.AreaId,
                    inspectionCode,
                    request.Title,
                    request.InspectorUserId,
                    request.ScheduledDate,
                    request.Description,
                    _currentUser.UserId);

                _context.Inspections.Add(inspection);
                await _context.SaveChangesAsync(cancellationToken);

                // Notificar al inspector
                await _notificationService.SendNotificationAsync(new NotificationRequest
                {
                    UserId = request.InspectorUserId,
                    Subject = "Nueva inspección asignada",
                    Message = $"Se le ha asignado la inspección: {request.Title} para el {request.ScheduledDate:dd/MM/yyyy}",
                    Type = NotificationType.Info,
                    Channel = NotificationChannel.InApp | NotificationChannel.Email,
                    Data = new Dictionary<string, object>
                    {
                        ["InspectionId"] = inspection.Id,
                        ["AreaName"] = area.AreaName,
                        ["ScheduledDate"] = request.ScheduledDate
                    }
                });

                var dto = _mapper.Map<InspectionDto>(inspection);
                dto.ProgramName = program.ProgramName;
                dto.AreaName = area.AreaName;
                dto.InspectorName = inspector.FullName;

                _logger.LogInformation(
                    "Inspección creada: {InspectionId} para área {AreaId} por usuario {UserId}",
                    inspection.Id, request.AreaId, _currentUser.UserId);

                return Result<InspectionDto>.Success(dto, "Inspección creada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear inspección");
                return Result<InspectionDto>.Failure("Error al crear la inspección");
            }
        }

        private async Task<string> GenerateInspectionCode(CancellationToken cancellationToken)
        {
            var date = DateTime.UtcNow;
            var prefix = $"INS-{date:yyyyMM}";

            var lastInspection = await _context.Inspections
                .Where(i => i.InspectionCode.StartsWith(prefix))
                .OrderByDescending(i => i.InspectionCode)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastInspection == null)
            {
                return $"{prefix}-0001";
            }

            var lastNumber = int.Parse(lastInspection.InspectionCode.Split('-').Last());
            return $"{prefix}-{(lastNumber + 1):D4}";
        }
    }
}