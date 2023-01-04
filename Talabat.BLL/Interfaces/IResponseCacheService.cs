namespace Talabat.BLL.Interfaces
{
    public interface IResponseCacheService
    {
        Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive);
        Task<string> GetCacheResponse(string cacheKey);

    }
}
