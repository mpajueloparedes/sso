using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MaproSSO.Application.Common.Interfaces;

namespace MaproSSO.Application.Common.Behaviours
{
    public class TenantFilterBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationDbContext _context;
        private readonly ILogger<TenantFilterBehaviour<TRequest, TResponse>> _logger;

        public TenantFilterBehaviour(
            ICurrentUserService currentUserService,
            IApplicationDbContext context,
            ILogger<TenantFilterBehaviour<TRequest, TResponse>> logger)
        {
            _currentUserService = currentUserService;
            _context = context;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            // Aplicar filtro de tenant si no es SuperAdmin
            if (!_currentUserService.IsSuperAdmin && _currentUserService.TenantId.HasValue)
            {
                try
                {
                    // Establecer el TenantId en el contexto de la sesión para Row Level Security
                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC sp_set_session_context @key=N'TenantId', @value=@p0",
                        _currentUserService.TenantId.Value.ToString());

                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC sp_set_session_context @key=N'IsSuperAdmin', @value=@p0",
                        "0");

                    _logger.LogDebug("Filtro de Tenant aplicado: {TenantId}", _currentUserService.TenantId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al aplicar filtro de tenant");
                    throw;
                }
            }
            else if (_currentUserService.IsSuperAdmin)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_set_session_context @key=N'IsSuperAdmin', @value=@p0",
                    "1");
            }

            return await next();
        }
    }
}