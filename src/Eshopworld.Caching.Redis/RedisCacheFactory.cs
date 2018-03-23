using System;
using Eshopworld.Caching.Core;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;

namespace Eshopworld.Caching.Redis
{
    public class RedisCacheFactory : ICacheFactory, IDisposable
    {
        private readonly StackExchange.Redis.Extensions.Core.ISerializer serializer;
        private readonly ConnectionMultiplexer redisConnection;

        public RedisCacheFactory(string connectionString)
        {
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

#if DEBUG
            this.serializer = new StackExchange.Redis.Extensions.Newtonsoft.NewtonsoftSerializer();
#else
            // todo: choose alternative 'binary' serializer
            this.serializer = new StackExchange.Redis.Extensions.Newtonsoft.NewtonsoftSerializer();
#endif

            redisConnection = ConnectionMultiplexer.Connect(connectionString);
        }

        public ICache<T> CreateDefault<T>() => Create<T>(typeof(T).Name);

        public ICache<T> Create<T>(string name) => new RedisCache<T>(new StackExchangeRedisCacheClient(redisConnection, serializer));

        public void Dispose() => redisConnection.Dispose();
    }
}