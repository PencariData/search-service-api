using Microsoft.Extensions.Caching.Memory;

namespace SearchService.Shared.Extensions;

public static class CacheExtensions
{
    public static void SetWithConfig<T>(
        this IMemoryCache cache,
        string key,
        T value,
        int durationMinutes)
    {
        cache.Set(
            key, 
            value,   
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(durationMinutes)
            } );
    }
}