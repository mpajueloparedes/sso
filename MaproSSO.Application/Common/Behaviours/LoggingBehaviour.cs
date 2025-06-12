using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using MaproSSO.Application.Common.Interfaces;

namespace MaproSSO.Application.Common.Behaviours
{
    public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
        where TRequest : notnull
    {
        private readonly ILogger<LoggingBehaviour<TRequest>> _logger;
        private readonly ICurrentUserService _currentUserService;

        public LoggingBehaviour(
            ILogger<LoggingBehaviour<TRequest>> logger,
            ICurrentUserService currentUserService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId;
            var userName = _currentUserService.UserName;
            var tenantId = _currentUserService.TenantId;

            _logger.LogInformation(
                "MaproSSO Request: {Name} {@UserId} {@UserName} {@TenantId} {@Request}",
                requestName, userId, userName, tenantId, request);

            await Task.CompletedTask;
        }
    }
}