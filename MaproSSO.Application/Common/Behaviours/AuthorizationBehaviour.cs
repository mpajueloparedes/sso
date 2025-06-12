using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using MaproSSO.Application.Common.Attributes;
using MaproSSO.Application.Common.Exceptions;
using MaproSSO.Application.Common.Interfaces;

namespace MaproSSO.Application.Common.Behaviours
{
    public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<AuthorizationBehaviour<TRequest, TResponse>> _logger;

        public AuthorizationBehaviour(
            ICurrentUserService currentUserService,
            ILogger<AuthorizationBehaviour<TRequest, TResponse>> logger)
        {
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var authorizeAttributes = request.GetType()
                .GetCustomAttributes<AuthorizeAttribute>()
                .ToArray();

            if (authorizeAttributes.Any())
            {
                // Verificar si el usuario está autenticado
                if (!_currentUserService.IsAuthenticated)
                {
                    _logger.LogWarning("Usuario no autenticado intentando acceder a {RequestType}",
                        request.GetType().Name);
                    throw new UnauthorizedAccessException("Usuario no autenticado");
                }

                // Si es SuperAdmin, tiene acceso a todo
                if (_currentUserService.IsSuperAdmin)
                {
                    return await next();
                }

                foreach (var attribute in authorizeAttributes)
                {
                    var authorized = true;

                    // Verificar roles
                    if (!string.IsNullOrWhiteSpace(attribute.Roles))
                    {
                        var roles = attribute.Roles.Split(',').Select(r => r.Trim()).ToArray();

                        if (attribute.RequireAll)
                        {
                            authorized = roles.All(role => _currentUserService.HasRole(role));
                        }
                        else
                        {
                            authorized = _currentUserService.HasAnyRole(roles);
                        }
                    }

                    // Verificar permisos
                    if (authorized && !string.IsNullOrWhiteSpace(attribute.Permissions))
                    {
                        var permissions = attribute.Permissions.Split(',').Select(p => p.Trim()).ToArray();

                        if (attribute.RequireAll)
                        {
                            authorized = _currentUserService.HasAllPermissions(permissions);
                        }
                        else
                        {
                            authorized = _currentUserService.HasAnyPermission(permissions);
                        }
                    }

                    if (!authorized)
                    {
                        _logger.LogWarning(
                            "Usuario {UserId} sin permisos para acceder a {RequestType}. Roles requeridos: {Roles}, Permisos requeridos: {Permissions}",
                            _currentUserService.UserId,
                            request.GetType().Name,
                            attribute.Roles,
                            attribute.Permissions);

                        throw new ForbiddenAccessException("No tiene los permisos necesarios para realizar esta operación");
                    }
                }
            }

            return await next();
        }
    }
}