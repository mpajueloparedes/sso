using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Exceptions;
using MaproSSO.Domain.Entities.Tenant;

namespace MaproSSO.Application.Features.Tenants.Commands.DeactivateTenant
{
    public class DeactivateTenantCommandHandler : IRequestHandler<DeactivateTenantCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<DeactivateTenantCommandHandler> _logger;

        public DeactivateTenantCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            ILogger<DeactivateTenantCommandHandler> logger)
        {
            _context = context;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<r> Handle(DeactivateTenantCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var tenant = await _context.Tenants
                    .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

                if (tenant == null)
                {
                    throw new NotFoundException(nameof(Tenant), request.Id);
                }

                tenant.Deactivate(_currentUser.UserId.Value);

                // Desactivar todos los usuarios del tenant
                var users = await _context.Users
                    .Where(u => u.TenantId == tenant.Id && u.IsActive)
                    .ToListAsync(cancellationToken);

                foreach (var user in users)
                {
                    user.Deactivate(_currentUser.UserId.Value);
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogWarning("Tenant desactivado: {TenantId} - {CompanyName} por usuario {UserId}. Razón: {Reason}",
                    tenant.Id, tenant.CompanyName, _currentUser.UserId, request.Reason);

                return Result.Success("Empresa desactivada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desactivar tenant {TenantId}", request.Id);
                throw;
            }
        }
    }
}