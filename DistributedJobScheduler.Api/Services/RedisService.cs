using DistributedJobScheduler.Api.Config;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;

namespace DistributedJobScheduler.Api.Services
{
    public class RedisService : IDisposable
    {
        private readonly RedisConfig _redisConfig;
        private readonly ConnectionMultiplexer _redis; // Changed from IConnectionMultiplexer to ConnectionMultiplexer

        public RedisService(IOptions<RedisConfig> redisConfig)
        {
            if (redisConfig?.Value == null || string.IsNullOrWhiteSpace(redisConfig.Value.ConnectionString))
            {
                throw new ArgumentNullException(nameof(redisConfig), "Redis connection string is missing or empty.");
            }

            _redisConfig = redisConfig.Value;
            _redis = ConnectionMultiplexer.Connect(_redisConfig.ConnectionString);
        }

        public async Task PublishMessageAsync(string channel, string message)
        {
            if (string.IsNullOrWhiteSpace(channel) || string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Channel and message cannot be null or empty.");
            }

            var pubSub = _redis.GetSubscriber();
            await pubSub.PublishAsync(RedisChannel.Literal($"{_redisConfig.ChannelPrefix}:{channel}"), message);
        }

        // Properly implement IDisposable
        public void Dispose()
        {
            _redis.Dispose();
            GC.SuppressFinalize(this); // Ensures derived types don’t need to reimplement Dispose
        }
    }
}
