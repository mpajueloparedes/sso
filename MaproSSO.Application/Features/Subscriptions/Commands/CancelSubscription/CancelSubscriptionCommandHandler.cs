using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Exceptions;
using MaproSSO.Domain.Entities.Subscription;

namespace MaproSSO.Application.Features.Subscriptions.Commands.CancelSubscription
{
    public class CancelSubscriptionCommandHandler : IRequestHandler<CancelSubscriptionCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IEmailService _emailService;
        private readonly ILogger<CancelSubscriptionCommandHandler> _logger;

        public CancelSubscriptionCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IEmailService emailService,
            ILogger<CancelSubscriptionCommandHandler> logger)
        {
            _context = context;
            _currentUser = currentUser;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<r> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var subscription = await _context.Subscriptions
                    .Include(s => s.Plan)
                    .FirstOrDefaultAsync(s => s.Id == request.SubscriptionId, cancellationToken);

                if (subscription == null)
                {
                    throw new NotFoundException(nameof(Subscription), request.SubscriptionId);
                }

                // Verificar permisos
                if (!_currentUser.IsSuperAdmin && subscription.TenantId != _currentUser.TenantId)
                {
                    throw new ForbiddenAccessException("No tiene permisos para cancelar esta suscripción");
                }

                // Obtener información del tenant
                var tenant = await _context.Tenants
                    .FirstOrDefaultAsync(t => t.Id == subscription.TenantId, cancellationToken);

                // Cancelar suscripción
                subscription.Cancel(request.Reason, _currentUser.UserId.Value);

                if (request.ImmediateCancellation)
                {
                    // Suspender inmediatamente
                    subscription.Suspend("Cancelación inmediata", _currentUser.UserId.Value);
                }
                else
                {
                    // Desactivar auto-renovación
                    subscription.ToggleAutoRenew(_currentUser.UserId.Value);
                }

                await _context.SaveChangesAsync(cancellationToken);

                // Enviar email de confirmación
                await _emailService.SendTemplateAsync(
                    "SubscriptionCancelled",
                    tenant.Email.Value,
                    new Dictionary<string, object>
                    {
                        ["CompanyName"] = tenant.CompanyName,
                        ["PlanName"] = subscription.Plan.PlanName,
                        ["EndDate"] = subscription.EndDate,
                        ["Reason"] = request.Reason,
                        ["IsImmediate"] = request.ImmediateCancellation
                    });

                _logger.LogWarning(
                    "Suscripción cancelada: {SubscriptionId} para tenant {TenantId}. Razón: {Reason}",
                    subscription.Id, subscription.TenantId, request.Reason);

                return Result.Success(request.ImmediateCancellation
                    ? "Suscripción cancelada inmediatamente"
                    : "Suscripción se cancelará al final del período actual");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar suscripción {SubscriptionId}", request.SubscriptionId);
                throw;
            }
        }
    }
}