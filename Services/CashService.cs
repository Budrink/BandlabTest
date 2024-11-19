using Microsoft.Extensions.Caching.Memory;

namespace ImageCommentApp.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        // Получить данные из кэша
        public Task<T?> GetAsync<T>(string key)
        {
            return Task.FromResult(_memoryCache.TryGetValue(key, out T value) ? value : default);
        }

        // Сохранить данные в кэше
        public Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            _memoryCache.Set(key, value, expiration);
            return Task.CompletedTask;
        }
    }
}