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
using MaproSSO.Application.Common.Exceptions;
using MaproSSO.Domain.Enums;
using MaproSSO.Application.Common.Services;

namespace MaproSSO.Application.Features.Subscriptions.Queries.GetPaymentHistory
{
    public class GetPaymentHistoryQueryHandler : IRequestHandler<GetPaymentHistoryQuery, Result<PaginatedList<PaymentDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public GetPaymentHistoryQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            ICurrentUserService currentUser)
        {
            _context = context;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<Result<PaginatedList<PaymentDto>>> Handle(
            GetPaymentHistoryQuery request,
            CancellationToken cancellationToken)
        {
            var query = _context.Payments
                .Include(p => p.Subscription)
                .AsQueryable();

            // Aplicar filtros según permisos
            if (!_currentUser.IsSuperAdmin)
            {
                if (request.TenantId.HasValue && request.TenantId.Value != _currentUser.TenantId)
                {
                    throw new ForbiddenAccessException("No tiene permisos para ver estos pagos");
                }

                query = query.Where(p => p.Subscription.TenantId == _currentUser.TenantId);
            }
            else if (request.TenantId.HasValue)
            {
                query = query.Where(p => p.Subscription.TenantId == request.TenantId.Value);
            }

            if (request.SubscriptionId.HasValue)
            {
                query = query.Where(p => p.SubscriptionId == request.SubscriptionId.Value);
            }

            if (request.DateFrom.HasValue)
            {
                query = query.Where(p => p.PaymentDate >= request.DateFrom.Value);
            }

            if (request.DateTo.HasValue)
            {
                query = query.Where(p => p.PaymentDate <= request.DateTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (Enum.TryParse<PaymentStatus>(request.Status, out var status))
                {
                    query = query.Where(p => p.Status == status);
                }
            }

            var paginatedList = await query
                .OrderByDescending(p => p.PaymentDate)
                .ProjectTo<PaymentDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize);

            return Result<PaginatedList<PaymentDto>>.Success(paginatedList);
        }
    }
}