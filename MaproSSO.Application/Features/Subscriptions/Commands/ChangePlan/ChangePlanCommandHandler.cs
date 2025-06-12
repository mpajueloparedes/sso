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
using MaproSSO.Domain.Enums;
using MaproSSO.Application.Common.Services;
using MaproSSO.Domain.Entities.Subscription;

namespace MaproSSO.Application.Features.Subscriptions.Commands.ChangePlan
{
    public class ChangePlanCommandHandler : IRequestHandler<ChangePlanCommand, Result<SubscriptionDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<ChangePlanCommandHandler> _logger;

        public ChangePlanCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            ICurrentUserService currentUser,
            ILogger<ChangePlanCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<Result<SubscriptionDto>> Handle(
            ChangePlanCommand request,
            CancellationToken cancellationToken)
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
                    throw new ForbiddenAccessException("No tiene permisos para cambiar este plan");
                }

                if (subscription.Status != SubscriptionStatus.Active)
                {
                    return Result<SubscriptionDto>.Failure("Solo se pueden cambiar planes de suscripciones activas");
                }

                // Verificar que el nuevo plan existe
                var newPlan = await _context.Plans
                    .Include(p => p.Features)
                    .FirstOrDefaultAsync(p => p.Id == request.NewPlanId && p.IsActive, cancellationToken);

                if (newPlan == null)
                {
                    return Result<SubscriptionDto>.Failure("Plan no válido");
                }

                var newBillingCycle = Enum.Parse<BillingCycle>(request.NewBillingCycle);

                // Cambiar plan
                subscription.ChangePlan(request.NewPlanId, newBillingCycle, _currentUser.UserId.Value);

                // Actualizar límites de características
                var currentUsages = await _context.FeatureUsages
                    .Where(fu => fu.TenantId == subscription.TenantId)
                    .ToListAsync(cancellationToken);

                foreach (var feature in newPlan.Features.Where(f => f.FeatureType == "Limit"))
                {
                    var usage = currentUsages.FirstOrDefault(u => u.FeatureCode == feature.FeatureCode);

                    if (usage != null)
                    {
                        usage.UpdateLimit(feature.GetFeatureLimit());
                    }
                    else
                    {
                        var newUsage = FeatureUsage.Create(
                            subscription.TenantId,
                            feature.FeatureCode,
                            feature.GetFeatureLimit(),
                            "Monthly");

                        _context.FeatureUsages.Add(newUsage);
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);

                var dto = _mapper.Map<SubscriptionDto>(subscription);
                dto.PlanName = newPlan.PlanName;

                _logger.LogInformation(
                    "Plan cambiado para suscripción {SubscriptionId} de {OldPlanId} a {NewPlanId}",
                    subscription.Id, subscription.Plan.Id, request.NewPlanId);

                return Result<SubscriptionDto>.Success(dto, "Plan cambiado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar plan de suscripción {SubscriptionId}", request.SubscriptionId);
                throw;
            }
        }
    }
}
