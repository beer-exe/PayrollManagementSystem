using MediatR;
using Microsoft.Extensions.Logging;
using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Behaviours;

public class CacheInvalidationBehavior<TRequest, TResponse>(
    ICacheService cacheService,
    ILogger<CacheInvalidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        if (request is ICacheInvalidatorCommand invalidatorCommand)
        {
            var prefixes = invalidatorCommand.CacheKeyPrefix.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var prefix in prefixes)
            {
                try
                {
                    await cacheService.RemoveByPrefixAsync(prefix, cancellationToken);
                    logger.LogInformation("Cache Invalidated for prefix -> '{Prefix}'", prefix);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Error clearing cache (Prefix: {Prefix}). Database was still updated successfully.", prefix);
                }
            }
        }

        return response;
    }
}
