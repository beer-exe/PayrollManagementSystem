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

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        if (value != null)
        {
            var options = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
            {
                options.SetAbsoluteExpiration(expiration.Value);
            }

            options.RegisterPostEvictionCallback((evictedKey, _, _, _) =>
                CacheKeys.TryRemove(evictedKey.ToString()!, out _));

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

    public Task ClearAllAsync(CancellationToken cancellationToken = default)
    {
        var allKeys = CacheKeys.Keys.ToList();
        foreach (var key in allKeys)
        {
            memoryCache.Remove(key);
            CacheKeys.TryRemove(key, out _);
        }

        return Task.CompletedTask;
    }
}
