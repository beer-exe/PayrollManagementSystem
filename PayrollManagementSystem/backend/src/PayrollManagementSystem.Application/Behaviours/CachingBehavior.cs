using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Behaviours;

public class CachingBehavior<TRequest, TResponse>(
    ICacheService cacheService,
    ILogger<CachingBehavior<TRequest, TResponse>> logger,
    IConfiguration configuration)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICacheableQuery cacheableQuery)
        {
            return await next();
        }

        var cacheKey = cacheableQuery.CacheKey;
        var cachedResponse = await cacheService.GetAsync<TResponse>(cacheKey, cancellationToken);

        if (cachedResponse != null)
        {
            logger.LogInformation("Fetched from Cache -> '{CacheKey}'", cacheKey);
            return cachedResponse;
        }

        var response = await next();

        var defaultExpiration = int.Parse(configuration["CacheSettings:DefaultExpirationInMinutes"] ?? "60");
        var expiration = cacheableQuery.Expiration ?? TimeSpan.FromMinutes(defaultExpiration);

        await cacheService.SetAsync(cacheKey, response, expiration, cancellationToken);
        logger.LogInformation("Added to Cache -> '{CacheKey}'", cacheKey);

        return response;
    }
}
