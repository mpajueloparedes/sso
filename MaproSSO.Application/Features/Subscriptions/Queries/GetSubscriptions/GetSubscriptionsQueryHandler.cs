using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Mappings;
using MaproSSO.Domain.Enums;

namespace MaproSSO.Application.Features.Subscriptions.Queries.GetSubscriptions
{
    public class GetSubscriptionsQueryHandler : IRequestHandler<GetSubscriptionsQuery, Result<PaginatedList<SubscriptionListDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetSubscriptionsQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PaginatedList<SubscriptionListDto>>> Handle(
            GetSubscriptionsQuery request,
            CancellationToken cancellationToken)
        {
            var query = _context.Subscriptions
                .Include(s => s.Plan)
                .AsQueryable();

            // Filtros
            if (request.TenantId.HasValue)
            {
                query = query.Where(s => s.TenantId == request.TenantId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (Enum.TryParse<SubscriptionStatus>(request.Status, out var status))
                {
                    query = query.Where(s => s.Status == status);
                }
            }

            if (!string.IsNullOrWhiteSpace(request.PlanType))
            {
                query = query.Where(s => s.Plan.PlanType.ToString() == request.PlanType);
            }

            if (request.ExpiringBefore.HasValue)
            {
                query = query.Where(s => s.EndDate <= request.ExpiringBefore.Value);
            }

            // Ordenamiento
            query = request.OrderBy switch
            {
                "EndDate" => request.OrderDescending
                    ? query.OrderByDescending(s => s.EndDate)
                    : query.OrderBy(s => s.EndDate),
                "CreatedAt" => request.OrderDescending
                    ? query.OrderByDescending(s => s.CreatedAt)
                    : query.OrderBy(s => s.CreatedAt),
                "PlanName" => request.OrderDescending
                    ? query.OrderByDescending(s => s.Plan.PlanName)
                    : query.OrderBy(s => s.Plan.PlanName),
                _ => query.OrderBy(s => s.EndDate)
            };

            var paginatedList = await query
                .ProjectTo<SubscriptionListDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize);

            return Result<PaginatedList<SubscriptionListDto>>.Success(paginatedList);
        }
    }
}