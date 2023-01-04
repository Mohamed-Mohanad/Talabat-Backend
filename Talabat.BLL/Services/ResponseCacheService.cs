using StackExchange.Redis;
using System.Text.Json;
using Talabat.BLL.Interfaces;

namespace Talabat.BLL.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDatabase _database;

        public ResponseCacheService(IConnectionMultiplexer redis) 
        {
            _database = redis.GetDatabase();
        }

        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            if (response == null) return;

            var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var serializedResponse = JsonSerializer.Serialize(response, options);

            await _database.StringSetAsync(cacheKey, serializedResponse, timeToLive);
        }

        public async Task<string> GetCacheResponse(string cacheKey)
        {
            var cachedResponse = await _database.StringGetAsync(cacheKey);

            if (cachedResponse.IsNullOrEmpty) return null;

            return cachedResponse;
        }
    }
}
