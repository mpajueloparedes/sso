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
using MaproSSO.Domain.Entities.Announcements;

namespace MaproSSO.Application.Features.SSO.Announcements.Queries.GetAnnouncementById
{
    public class GetAnnouncementByIdQueryHandler : IRequestHandler<GetAnnouncementByIdQuery, Result<AnnouncementDetailDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public GetAnnouncementByIdQueryHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IMapper mapper)
        {
            _context = context;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<Result<AnnouncementDetailDto>> Handle(
            GetAnnouncementByIdQuery request,
            CancellationToken cancellationToken)
        {
            var announcement = await _context.Announcements
                .Include(a => a.Area)
                .Include(a => a.Images)
                .Include(a => a.CorrectiveActions)
                    .ThenInclude(ca => ca.ResponsibleUser)
                .Include(a => a.CorrectiveActions)
                    .ThenInclude(ca => ca.Evidences)
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (announcement == null)
            {
                throw new NotFoundException(nameof(Announcement), request.Id);
            }

            // Verificar permisos
            if (announcement.TenantId != _currentUser.TenantId)
            {
                throw new ForbiddenAccessException("No tiene permisos para ver este anuncio");
            }

            // Si no es admin, verificar que pertenece al área
            if (!_currentUser.HasRole("AdminSSO") && !_currentUser.HasRole("SuperAdmin"))
            {
                var userBelongsToArea = await _context.AreaUsers
                    .AnyAsync(au => au.AreaId == announcement.AreaId && au.UserId == _currentUser.UserId,
                        cancellationToken);

                if (!userBelongsToArea)
                {
                    throw new ForbiddenAccessException("No tiene permisos para ver este anuncio");
                }
            }

            // Obtener información del creador
            var creator = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == announcement.CreatedBy, cancellationToken);

            var dto = _mapper.Map<AnnouncementDetailDto>(announcement);
            dto.CreatedByName = creator?.FullName;
            dto.CompletionPercentage = announcement.GetCompletionPercentage();

            return Result<AnnouncementDetailDto>.Success(dto);
        }
    }
}