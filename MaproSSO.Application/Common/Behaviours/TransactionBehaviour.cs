using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MaproSSO.Application.Common.Interfaces;

namespace MaproSSO.Application.Common.Behaviours
{
    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;

        public TransactionBehaviour(
            IApplicationDbContext context,
            ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            // Solo aplicar transacción a Commands, no a Queries
            if (request.GetType().Name.EndsWith("Query"))
            {
                return await next();
            }

            // Si ya hay una transacción activa, no crear una nueva
            if (_context.Database.CurrentTransaction != null)
            {
                return await next();
            }

            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    _logger.LogDebug("Iniciando transacción para {RequestType}", request.GetType().Name);

                    var response = await next();

                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    _logger.LogDebug("Transacción completada exitosamente para {RequestType}", request.GetType().Name);

                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en transacción para {RequestType}", request.GetType().Name);
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }
    }
}