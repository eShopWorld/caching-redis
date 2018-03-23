using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eshopworld.Caching.Core;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;

namespace Eshopworld.Caching.Redis
{
    public class RedisCache<T> : IDistributedCache<T>
    {
        private readonly ICacheClient client;
        private readonly string keyPrefix;

        public IDatabase Database => client.Database;

        public RedisCache(ICacheClient cacheClient)
        {
            this.client = cacheClient;
            keyPrefix = typeof(T).Name + ":";
        }

        public T Add(CacheItem<T> item)
        {
            client.Add(keyPrefix + item.Key, item.Value, item.Duration);

            return item.Value;
        }

        public async Task<T> AddAsync(CacheItem<T> item)
        {
            await client.AddAsync(keyPrefix + item.Key, item.Value, item.Duration);

           

            return item.Value;
        }


        public void Set(CacheItem<T> item) => Add(item);
        public Task SetAsync(CacheItem<T> item) => AddAsync(item);


        public bool Exists(string key) => client.Exists(keyPrefix + key);
        public Task<bool> ExistsAsync(string key) => client.ExistsAsync(keyPrefix + key);
        public async Task<bool> ExistsAsync2(string key) => await client.ExistsAsync(keyPrefix + key);

        public bool KeyExpire(string key, TimeSpan? expiry) => client.Database.KeyExpire(keyPrefix + key, expiry);
        public Task<bool> KeyExpireAsync(string key, TimeSpan? expiry) => client.Database.KeyExpireAsync(keyPrefix + key, expiry);

        public void Remove(string key) => client.Remove(keyPrefix + key);
        public Task RemoveAsync(string key) => client.RemoveAsync(keyPrefix + key);

        public Task<T> GetAsync(string key) => client.GetAsync<T>(keyPrefix + key);
        public T Get(string key) => client.Get<T>(keyPrefix + key);

        public CacheResult<T> GetResult(string key)
        {
            var redisValue = client.Database.StringGet(keyPrefix + key);

            return !redisValue.HasValue ? CacheResult<T>.Miss() : new CacheResult<T>(true, client.Serializer.Deserialize<T>(redisValue));
        }

        public async Task<CacheResult<T>> GetResultAsync(string key)
        {
            var redisValue = await client.Database.StringGetAsync(keyPrefix + key);

            return !redisValue.HasValue ? CacheResult<T>.Miss() : new CacheResult<T>(true, client.Serializer.Deserialize<T>(redisValue));
        }

        public IEnumerable<KeyValuePair<string, T>> Get(IEnumerable<string> keys)
        {
            if (true)
            {
                return keys.Select(k => new KeyValuePair<string, T>(k, client.Get<T>(keyPrefix + k))).ToArray();
            }
            else
            {
                // https://github.com/StackExchange/StackExchange.Redis/issues/448
                // this can throw an ExecutionEngineException on the StringGet portion of the method below, hence the above workaround
                var prefixedKeys = keys.Select(key => keyPrefix + key);
                var results = client.GetAll<T>(prefixedKeys);

                return results.Select(kvp => new KeyValuePair<string, T>(kvp.Key.Split(':')[1], kvp.Value));
            }
        }

        public async Task<IEnumerable<KeyValuePair<string, T>>> GetAsync(IEnumerable<string> keys)
        {
            var prefixedKeys = keys.Select(key => keyPrefix + key);
            var results = await client.GetAllAsync<T>(prefixedKeys);

            return results.Select(kvp => new KeyValuePair<string, T>(kvp.Key.Split(':')[1], kvp.Value));
        }
    }
}
