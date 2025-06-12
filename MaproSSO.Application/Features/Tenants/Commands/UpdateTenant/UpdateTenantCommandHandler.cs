using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Exceptions;
using MaproSSO.Domain.ValueObjects;
using MaproSSO.Domain.Entities.Tenant;

namespace MaproSSO.Application.Features.Tenants.Commands.UpdateTenant
{
    public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<UpdateTenantCommandHandler> _logger;

        public UpdateTenantCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            ILogger<UpdateTenantCommandHandler> logger)
        {
            _context = context;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var tenant = await _context.Tenants
                    .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

                if (tenant == null)
                {
                    throw new NotFoundException(nameof(Tenant), request.Id);
                }

                // Verificar permisos - solo SuperAdmin puede editar cualquier tenant
                if (!_currentUser.IsSuperAdmin && tenant.Id != _currentUser.TenantId)
                {
                    throw new ForbiddenAccessException("No tiene permisos para editar esta empresa");
                }

                var address = Address.Create(
                    request.Address.Country,
                    request.Address.State,
                    request.Address.City,
                    request.Address.Street,
                    request.Address.PostalCode);

                tenant.Update(
                    request.CompanyName,
                    request.TradeName,
                    request.Industry,
                    request.EmployeeCount,
                    address,
                    request.Phone,
                    request.Website,
                    _currentUser.UserId.Value);

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Tenant actualizado: {TenantId} por usuario {UserId}",
                    request.Id, _currentUser.UserId);

                return Result.Success("Empresa actualizada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar tenant {TenantId}", request.Id);
                throw;
            }
        }
    }
}