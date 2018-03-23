using System;
using StackExchange.Redis;
using Xunit;

namespace Eshopworld.Caching.Redis.Tests.Integration
{
    public class RedisCacheFactoryTests
    {
        private const string skip = "DISABLED";

        [Fact]
        public void NewInstace_WithInvalidConnectionString_ThrowsException()
        {
            // Arrange
            // Act

            Assert.Throws<RedisConnectionException>((Action)(() => new RedisCacheFactory("1.1.1.1:6379"))); // this is a fake address
            // Assert
        }

        [Fact(Skip = skip)]
        public void NewInstace_WithValidConnectionString_NoException()
        {
            // Arrange
            //RedisCacheFactory factory;
            // Act
            using (new RedisCacheFactory(Environment.RedisConnectionString)) ;
            // Assert
        }

        [Fact(Skip = skip)]
        public void Create_RedisCacheFactory_NoException()
        {
            // Arrange
            using (var factory = new RedisCacheFactory(Environment.RedisConnectionString))
            {
                // Act
                var instance = factory.Create<SimpleObject>("testCache");

                // Assert
                Assert.IsType<RedisCache<SimpleObject>>(instance);
            }
        }
    }
}
