using Microsoft.Extensions.Caching.Memory;
using PayrollManagementSystem.Application.Common.Interfaces;
using System.Collections.Concurrent;

namespace PayrollManagementSystem.Infrastructure.Services;

public class MemoryCacheService(IMemoryCache memoryCache) : ICacheService
{
    private static readonly ConcurrentDictionary<string, bool> CacheKeys = new();

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        memoryCache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default)
    {
        if (value != null)
        {
            var options = new MemoryCacheEntryOptions();
            if (slidingExpiration.HasValue)
            {
                options.SetSlidingExpiration(slidingExpiration.Value);
            }
            
            memoryCache.Set(key, value, options);
            CacheKeys.TryAdd(key, true);
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        memoryCache.Remove(key);
        CacheKeys.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
    {
        var keysToRemove = CacheKeys.Keys.Where(k => k.StartsWith(prefixKey, StringComparison.OrdinalIgnoreCase)).ToList();

        foreach (var key in keysToRemove)
        {
            memoryCache.Remove(key);
            CacheKeys.TryRemove(key, out _);
        }

        return Task.CompletedTask;
    }
}
