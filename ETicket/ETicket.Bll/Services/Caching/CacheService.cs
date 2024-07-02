using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections.Concurrent;


namespace ETicket.Bll.Services.Caching
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly CacheConfiguration _cacheConfiguration;
        private static readonly ConcurrentDictionary<string, bool> CacheKeys = new();

        public CacheService(IDistributedCache cache, CacheConfiguration cacheConfiguration)
        {
            _distributedCache = cache;
            _cacheConfiguration = cacheConfiguration;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            string? cachedValue = await _distributedCache.GetStringAsync(key, cancellationToken);
            if (cachedValue is null)
            {
                return null;
            }

            T? value = JsonConvert.DeserializeObject<T>(cachedValue);

            return value;
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);

            CacheKeys.TryRemove(key, out bool _);
        }

        public async Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
        {
            IEnumerable<Task> tasks = CacheKeys
                .Keys
                .Where(k => k.StartsWith(prefixKey))
                .Select(k => RemoveAsync(k, cancellationToken));

            await Task.WhenAll(tasks);
        }

        public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
        {
            string cacheValue = JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });

            await _distributedCache.SetStringAsync(key, cacheValue, new DistributedCacheEntryOptions
            {
                SlidingExpiration = _cacheConfiguration.SlidingExpirationTimeSpan / 2
            }, cancellationToken);

            CacheKeys.TryAdd(key, false);
        }
    }
}
