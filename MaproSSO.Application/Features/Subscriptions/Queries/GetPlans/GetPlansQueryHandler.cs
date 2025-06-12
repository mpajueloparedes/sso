using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Common.Models;

namespace MaproSSO.Application.Features.Subscriptions.Queries.GetPlans
{
    public class GetPlansQueryHandler : IRequestHandler<GetPlansQuery, Result<List<PlanDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetPlansQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<List<PlanDto>>> Handle(
            GetPlansQuery request,
            CancellationToken cancellationToken)
        {
            var query = _context.Plans
                .Include(p => p.Features)
                .AsQueryable();

            if (request.OnlyActive)
            {
                query = query.Where(p => p.IsActive);
            }

            var plans = await query
                .OrderBy(p => p.SortOrder)
                .ToListAsync(cancellationToken);

            var planDtos = _mapper.Map<List<PlanDto>>(plans);

            return Result<List<PlanDto>>.Success(planDtos);
        }
    }
}