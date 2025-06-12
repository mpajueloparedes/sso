using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Exceptions;
using MaproSSO.Domain.Entities.Subscription;

namespace MaproSSO.Application.Features.Subscriptions.Queries.GetSubscriptionById
{
    public class GetSubscriptionByIdQueryHandler : IRequestHandler<GetSubscriptionByIdQuery, Result<SubscriptionDetailDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public GetSubscriptionByIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            ICurrentUserService currentUser)
        {
            _context = context;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<Result<SubscriptionDetailDto>> Handle(
            GetSubscriptionByIdQuery request,
            CancellationToken cancellationToken)
        {
            var subscription = await _context.Subscriptions
                .Include(s => s.Plan)
                    .ThenInclude(p => p.Features)
                .Include(s => s.Payments)
                .Include(s => s.History)
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (subscription == null)
            {
                throw new NotFoundException(nameof(Subscription), request.Id);
            }

            // Verificar permisos
            if (!_currentUser.IsSuperAdmin && subscription.TenantId != _currentUser.TenantId)
            {
                throw new ForbiddenAccessException("No tiene permisos para ver esta suscripción");
            }

            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.Id == subscription.TenantId, cancellationToken);

            var dto = _mapper.Map<SubscriptionDetailDto>(subscription);
            dto.TenantName = tenant?.CompanyName;

            // Agregar información de uso de características
            var featureUsages = await _context.FeatureUsages
                .Where(fu => fu.TenantId == subscription.TenantId)
                .ToListAsync(cancellationToken);

            dto.FeatureUsages = subscription.Plan.Features
                .Where(f => f.FeatureType == "Limit")
                .Select(f =>
                {
                    var usage = featureUsages.FirstOrDefault(fu => fu.FeatureCode == f.FeatureCode);
                    return new FeatureUsageDto
                    {
                        FeatureName = f.FeatureName,
                        FeatureCode = f.FeatureCode,
                        CurrentUsage = usage?.CurrentUsage ?? 0,
                        Limit = usage?.UsageLimit ?? f.GetFeatureLimit(),
                        UsagePercentage = usage?.GetUsagePercentage() ?? 0,
                        ResetPeriod = usage?.ResetPeriod ?? "Monthly"
                    };
                })
                .ToList();

            return Result<SubscriptionDetailDto>.Success(dto);
        }
    }
}
