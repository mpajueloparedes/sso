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
using MaproSSO.Domain.Entities.Tenant;

namespace MaproSSO.Application.Features.Tenants.Queries.GetTenantById
{
    public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, Result<TenantDetailDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public GetTenantByIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            ICurrentUserService currentUser)
        {
            _context = context;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<Result<TenantDetailDto>> Handle(
            GetTenantByIdQuery request,
            CancellationToken cancellationToken)
        {
            // Verificar permisos
            if (!_currentUser.IsSuperAdmin && request.Id != _currentUser.TenantId)
            {
                throw new ForbiddenAccessException("No tiene permisos para ver esta empresa");
            }

            var tenant = await _context.Tenants
                .Include(t => t.Settings)
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (tenant == null)
            {
                throw new NotFoundException(nameof(Tenant), request.Id);
            }

            // Obtener información adicional
            var subscription = await _context.Subscriptions
                .Include(s => s.Plan)
                    .ThenInclude(p => p.Features)
                .Where(s => s.TenantId == tenant.Id)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            var userCount = await _context.Users
                .CountAsync(u => u.TenantId == tenant.Id && u.IsActive, cancellationToken);

            var areaCount = await _context.Areas
                .CountAsync(a => a.TenantId == tenant.Id && a.IsActive, cancellationToken);

            var tenantDto = _mapper.Map<TenantDetailDto>(tenant);

            tenantDto.Subscription = subscription != null ? new SubscriptionSummaryDto
            {
                PlanName = subscription.Plan.PlanName,
                Status = subscription.Status.ToString(),
                EndDate = subscription.EndDate,
                DaysRemaining = subscription.GetDaysRemaining(),
                Features = subscription.Plan.Features.Select(f => new FeatureSummaryDto
                {
                    Name = f.FeatureName,
                    Code = f.FeatureCode,
                    Value = f.Value
                }).ToList()
            } : null;

            tenantDto.Statistics = new TenantStatisticsDto
            {
                UserCount = userCount,
                AreaCount = areaCount,
                AnnouncementCount = await _context.Announcements
                    .CountAsync(a => a.TenantId == tenant.Id, cancellationToken),
                InspectionCount = await _context.Inspections
                    .CountAsync(i => i.TenantId == tenant.Id, cancellationToken),
                TrainingCount = await _context.Trainings
                    .CountAsync(t => t.TenantId == tenant.Id, cancellationToken),
                AccidentCount = await _context.Accidents
                    .CountAsync(a => a.TenantId == tenant.Id, cancellationToken)
            };

            return Result<TenantDetailDto>.Success(tenantDto);
        }
    }
}