//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using AutoMapper;
//using MaproSSO.Application.Common.Interfaces;
//using MaproSSO.Application.Common.Models;
//using MaproSSO.Application.Common.Exceptions;
//using MaproSSO.Application.Features.Tenants.DTOs;

//namespace MaproSSO.Application.Features.Tenants.Queries.GetTenantSettings
//{
//    public class GetTenantSettingsQueryHandler : IRequestHandler<GetTenantSettingsQuery, Result<List<TenantSettingDto>>>
//    {
//        private readonly IApplicationDbContext _context;
//        private readonly IMapper _mapper;
//        private readonly ICurrentUserService _currentUser;

//        public GetTenantSettingsQueryHandler(
//            IApplicationDbContext context,
//            IMapper mapper,
//            ICurrentUserService currentUser)
//        {
//            _context = context;
//            _mapper = mapper;
//            _currentUser = currentUser;
//        }

//        public async Task<Result<List<TenantSettingDto>>> Handle(
//            GetTenantSettingsQuery request,
//            CancellationToken cancellationToken)
//        {
//            // Verificar permisos
//            if (!_currentUser.IsSuperAdmin && request.TenantId != _currentUser.TenantId)
//            {
//                throw new ForbiddenAccessException("No tiene permisos para ver estas configuraciones");
//            }

//            var settings = await _context.TenantSettings
//                .Where(s => s.TenantId == request.TenantId)
//                .OrderBy(s => s.SettingKey)
//                .ToListAsync(cancellationToken);

//            var settingDtos = _mapper.Map<List<TenantSettingDto>>(settings);

//            return Result<List<TenantSettingDto>>.Success(settingDtos);
//        }
//    }
//}