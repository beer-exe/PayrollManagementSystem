using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Behaviours;

public class CachingBehavior<TRequest, TResponse>(ICacheService cacheService, ILogger<CachingBehavior<TRequest, TResponse>> logger, IConfiguration configuration) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICacheableQuery cacheableQuery)
        {
            return await next();
        }

        var cacheKey = cacheableQuery.CacheKey;
        try
        {
            var cachedResponse = await cacheService.GetAsync<TResponse>(cacheKey, cancellationToken);
            if (cachedResponse != null)
            {
                logger.LogInformation("Fetched from Cache -> '{CacheKey}'", cacheKey);
                return cachedResponse;
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error reading from cache (Key: {CacheKey}). Will continue to get data from Database.", cacheKey);
        }

        var response = await next();
        try
        {
            var defaultExpiration = int.Parse(configuration["CacheSettings:DefaultExpirationInMinutes"] ?? "60");
            var expiration = cacheableQuery.Expiration ?? TimeSpan.FromMinutes(defaultExpiration);

            await cacheService.SetAsync(cacheKey, response, expiration, cancellationToken);
            logger.LogInformation("Added to Cache -> '{CacheKey}'", cacheKey);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error adding to cache (Key: {CacheKey}). Data is still returned normally.", cacheKey);
        }

        return response;
    }
}
