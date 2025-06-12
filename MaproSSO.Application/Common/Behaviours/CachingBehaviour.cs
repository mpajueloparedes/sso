using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using MaproSSO.Application.Common.Attributes;
using MaproSSO.Application.Common.Interfaces;
using System.Reflection;

namespace MaproSSO.Application.Common.Behaviours
{
    public class CachingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ICacheService _cacheService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<CachingBehaviour<TRequest, TResponse>> _logger;

        public CachingBehaviour(
            ICacheService cacheService,
            ICurrentUserService currentUserService,
            ILogger<CachingBehaviour<TRequest, TResponse>> logger)
        {
            _cacheService = cacheService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var cacheAttribute = request.GetType().GetCustomAttribute<CacheAttribute>();

            if (cacheAttribute == null)
            {
                return await next();
            }

            var cacheKey = GenerateCacheKey(request, cacheAttribute);

            // Intentar obtener del cache
            var cachedValue = await _cacheService.GetAsync<TResponse>(cacheKey);

            if (cachedValue != null)
            {
                _logger.LogDebug("Respuesta obtenida del cache para {RequestType} con clave {CacheKey}",
                    request.GetType().Name, cacheKey);
                return cachedValue;
            }

            // No está en cache, ejecutar el handler
            var response = await next();

            // Guardar en cache
            var expiry = TimeSpan.FromSeconds(cacheAttribute.DurationInSeconds);
            await _cacheService.SetAsync(cacheKey, response, expiry);

            _logger.LogDebug("Respuesta guardada en cache para {RequestType} con clave {CacheKey}",
                request.GetType().Name, cacheKey);

            return response;
        }

        private string GenerateCacheKey(TRequest request, CacheAttribute cacheAttribute)
        {
            var keyBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(cacheAttribute.CacheKeyPrefix))
            {
                keyBuilder.Append($"{cacheAttribute.CacheKeyPrefix}:");
            }
            else
            {
                keyBuilder.Append($"{request.GetType().Name}:");
            }

            if (cacheAttribute.VaryByTenant && _currentUserService.TenantId.HasValue)
            {
                keyBuilder.Append($"T{_currentUserService.TenantId}:");
            }

            if (cacheAttribute.VaryByUser && _currentUserService.UserId.HasValue)
            {
                keyBuilder.Append($"U{_currentUserService.UserId}:");
            }

            // Serializar request para generar hash
            var requestJson = JsonSerializer.Serialize(request);
            var requestHash = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(
                    Encoding.UTF8.GetBytes(requestJson)));

            keyBuilder.Append(requestHash);

            return keyBuilder.ToString();
        }
    }
}
