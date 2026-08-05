using Microsoft.Extensions.Logging;
using PayrollManagementSystem.Application.Common.Interfaces;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace PayrollManagementSystem.Infrastructure.Services
{
    public class ResilientRedisCacheService : ICacheService
    {
        private readonly ICacheService _inner;
        private readonly ICacheService _fallback;
        private readonly ILogger<ResilientRedisCacheService> _logger;

        private readonly ResiliencePipeline _pipeline;

        public ResilientRedisCacheService(
            RedisCacheService inner,
            MemoryCacheService fallback,
            ILogger<ResilientRedisCacheService> logger)
        {
            _inner = inner;
            _fallback = fallback;
            _logger = logger;

            _pipeline = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 2,
                    Delay = TimeSpan.FromMilliseconds(200),
                    BackoffType = DelayBackoffType.Exponential,
                    OnRetry = args =>
                    {
                        _logger.LogWarning("Redis retry #{Attempt} after error: {Message}",
                            args.AttemptNumber + 1, args.Outcome.Exception?.Message);
                        return ValueTask.CompletedTask;
                    }
                })
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    SamplingDuration = TimeSpan.FromSeconds(30),
                    MinimumThroughput = 5,
                    FailureRatio = 0.5,
                    BreakDuration = TimeSpan.FromSeconds(30),
                    OnOpened = args =>
                    {
                        _logger.LogError("Redis circuit breaker OPENED. Switching to in-memory cache for {Duration}s.",
                            args.BreakDuration.TotalSeconds);
                        return ValueTask.CompletedTask;
                    },
                    OnClosed = args =>
                    {
                        _logger.LogInformation("Redis circuit breaker CLOSED. Redis is healthy again.");
                        return ValueTask.CompletedTask;
                    },
                    OnHalfOpened = args =>
                    {
                        _logger.LogInformation("Redis circuit breaker HALF-OPEN. Probing Redis...");
                        return ValueTask.CompletedTask;
                    }
                })
                .Build();
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _pipeline.ExecuteAsync(
                    async ct => await _inner.GetAsync<T>(key, ct),
                    cancellationToken);
            }
            catch (BrokenCircuitException)
            {
                return await _fallback.GetAsync<T>(key, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis GetAsync failed for key [{Key}], falling back to memory.", key);
                return await _fallback.GetAsync<T>(key, cancellationToken);
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            try
            {
                await _pipeline.ExecuteAsync(
                    async ct => await _inner.SetAsync(key, value, expiration, ct),
                    cancellationToken);
            }
            catch (BrokenCircuitException)
            {
                await _fallback.SetAsync(key, value, expiration, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis SetAsync failed for key [{Key}], falling back to memory.", key);
                await _fallback.SetAsync(key, value, expiration, cancellationToken);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                await _pipeline.ExecuteAsync(
                    async ct => await _inner.RemoveAsync(key, ct),
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis RemoveAsync failed for key [{Key}].", key);
            }

            await _fallback.RemoveAsync(key, cancellationToken);
        }

        public async Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
        {
            try
            {
                await _pipeline.ExecuteAsync(
                    async ct => await _inner.RemoveByPrefixAsync(prefixKey, ct),
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis RemoveByPrefixAsync failed for prefix [{Prefix}].", prefixKey);
            }

            await _fallback.RemoveByPrefixAsync(prefixKey, cancellationToken);
        }

        public async Task ClearAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _pipeline.ExecuteAsync(
                    async ct => await _inner.ClearAllAsync(ct),
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis ClearAllAsync failed.");
            }

            await _fallback.ClearAllAsync(cancellationToken);
        }
    }
}
