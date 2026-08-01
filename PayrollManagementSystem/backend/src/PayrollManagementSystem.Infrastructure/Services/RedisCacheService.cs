using Microsoft.Extensions.Configuration;
using PayrollManagementSystem.Application.Common.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace PayrollManagementSystem.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _db;
    private readonly int _defaultExpirationInMinutes;

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer, IConfiguration configuration)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _db = connectionMultiplexer.GetDatabase();
        _defaultExpirationInMinutes = int.TryParse(configuration["CacheSettings:DefaultExpirationInMinutes"], out var defaultExp) ? defaultExp : 60;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty)
            return default;

        return JsonSerializer.Deserialize<T>(value!);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default)
    {
        if (value == null) return;

        var json = JsonSerializer.Serialize(value);
        var expiration = slidingExpiration ?? TimeSpan.FromMinutes(_defaultExpirationInMinutes);

        await _db.StringSetAsync(key, json, expiration);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _db.KeyDeleteAsync(key);
    }

    public async Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
    {
        var endpoints = _connectionMultiplexer.GetEndPoints();
        foreach (var endpoint in endpoints)
        {
            var server = _connectionMultiplexer.GetServer(endpoint);

            var batch = new List<RedisKey>();
            await foreach (var key in server.KeysAsync(pattern: prefixKey + "*"))
            {
                batch.Add(key);
                if (batch.Count >= 100)
                {
                    await _db.KeyDeleteAsync(batch.ToArray());
                    batch.Clear();
                }
            }

            if (batch.Count > 0)
                await _db.KeyDeleteAsync(batch.ToArray());
        }
    }

    public async Task ClearAllAsync(CancellationToken cancellationToken = default)
    {
        var endpoints = _connectionMultiplexer.GetEndPoints();
        foreach (var endpoint in endpoints)
        {
            var server = _connectionMultiplexer.GetServer(endpoint);
            await server.FlushDatabaseAsync();
        }
    }
}
