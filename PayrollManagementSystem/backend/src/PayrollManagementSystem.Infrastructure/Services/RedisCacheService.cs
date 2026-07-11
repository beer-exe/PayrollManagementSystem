using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Redis Cache is not yet implemented.");
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Redis Cache is not yet implemented.");
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Redis Cache is not yet implemented.");
    }

    public Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Redis Cache is not yet implemented.");
    }
}
