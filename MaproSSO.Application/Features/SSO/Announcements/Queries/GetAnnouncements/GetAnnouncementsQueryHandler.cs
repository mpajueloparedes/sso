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

namespace MaproSSO.Application.Features.SSO.Announcements.Queries.GetAnnouncements
{
    public class GetAnnouncementsQueryHandler : IRequestHandler<GetAnnouncementsQuery, Result<PaginatedList<AnnouncementListDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public GetAnnouncementsQueryHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IMapper mapper)
        {
            _context = context;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<Result<PaginatedList<AnnouncementListDto>>> Handle(
            GetAnnouncementsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Announcements
                    .Include(a => a.Area)
                    .Include(a => a.Images)
                    .Include(a => a.CorrectiveActions)
                    .Where(a => a.TenantId == _currentUser.TenantId.Value)
                    .AsQueryable();

                // Si el usuario no es admin, solo ver anuncios de sus áreas
                if (!_currentUser.HasRole("AdminSSO") && !_currentUser.HasRole("SuperAdmin"))
                {
                    var userAreaIds = await _context.AreaUsers
                        .Where(au => au.UserId == _currentUser.UserId)
                        .Select(au => au.AreaId)
                        .ToListAsync(cancellationToken);

                    query = query.Where(a => userAreaIds.Contains(a.AreaId));
                }

                // Filtros
                if (request.AreaId.HasValue)
                {
                    query = query.Where(a => a.AreaId == request.AreaId.Value);
                }

                if (request.Status.HasValue)
                {
                    query = query.Where(a => a.Status == request.Status.Value);
                }

                if (request.Severity.HasValue)
                {
                    query = query.Where(a => a.Severity == request.Severity.Value);
                }

                if (request.Type.HasValue)
                {
                    query = query.Where(a => a.Type == request.Type.Value);
                }

                if (request.DateFrom.HasValue)
                {
                    query = query.Where(a => a.ReportedAt >= request.DateFrom.Value);
                }

                if (request.DateTo.HasValue)
                {
                    query = query.Where(a => a.ReportedAt <= request.DateTo.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    query = query.Where(a =>
                        a.Title.Contains(request.SearchTerm) ||
                        a.Description.Contains(request.SearchTerm) ||
                        a.Location.Contains(request.SearchTerm));
                }

                // Ordenamiento
                query = request.OrderBy switch
                {
                    "Title" => request.OrderDescending
                        ? query.OrderByDescending(a => a.Title)
                        : query.OrderBy(a => a.Title),
                    "Severity" => request.OrderDescending
                        ? query.OrderByDescending(a => a.Severity)
                        : query.OrderBy(a => a.Severity),
                    "Status" => request.OrderDescending
                        ? query.OrderByDescending(a => a.Status)
                        : query.OrderBy(a => a.Status),
                    "ReportedAt" => request.OrderDescending
                        ? query.OrderByDescending(a => a.ReportedAt)
                        : query.OrderBy(a => a.ReportedAt),
                    _ => request.OrderDescending
                        ? query.OrderByDescending(a => a.CreatedAt)
                        : query.OrderBy(a => a.CreatedAt)
                };

                var paginatedList = await query
                    .ProjectTo<AnnouncementListDto>(_mapper.ConfigurationProvider)
                    .PaginatedListAsync(request.PageNumber, request.PageSize);

                return Result<PaginatedList<AnnouncementListDto>>.Success(paginatedList);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}