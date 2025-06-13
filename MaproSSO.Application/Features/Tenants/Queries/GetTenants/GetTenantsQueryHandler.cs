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
using MaproSSO.Application.Features.Tenants.DTOs;

namespace MaproSSO.Application.Features.Tenants.Queries.GetTenants
{
    public class GetTenantsQueryHandler : IRequestHandler<GetTenantsQuery, Result<PaginatedList<TenantDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetTenantsQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PaginatedList<TenantDto>>> Handle(
            GetTenantsQuery request,
            CancellationToken cancellationToken)
        {
            var query = _context.Tenants
                .Include(t => t.Settings)
                .AsQueryable();

            // Filtros
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(t =>
                    t.CompanyName.Contains(request.SearchTerm) ||
                    t.TradeName.Contains(request.SearchTerm) ||
                    t.TaxId.Contains(request.SearchTerm) ||
                    t.Email.Value.Contains(request.SearchTerm));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(t => t.IsActive == request.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Industry))
            {
                query = query.Where(t => t.Industry == request.Industry);
            }

            // Ordenamiento
            query = request.OrderBy switch
            {
                "CompanyName" => request.OrderDescending
                    ? query.OrderByDescending(t => t.CompanyName)
                    : query.OrderBy(t => t.CompanyName),
                "CreatedAt" => request.OrderDescending
                    ? query.OrderByDescending(t => t.CreatedAt)
                    : query.OrderBy(t => t.CreatedAt),
                "EmployeeCount" => request.OrderDescending
                    ? query.OrderByDescending(t => t.EmployeeCount)
                    : query.OrderBy(t => t.EmployeeCount),
                _ => query.OrderBy(t => t.CompanyName)
            };

            var paginatedList = await query
                .ProjectTo<TenantDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize);

            return Result<PaginatedList<TenantDto>>.Success(paginatedList);
        }
    }
}