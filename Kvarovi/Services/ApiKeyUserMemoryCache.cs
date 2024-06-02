namespace Kvarovi.Services;

using Microsoft.Extensions.Caching.Memory;

public class ApiKeyUserMemoryCache
{
    public MemoryCache Cache { get; } = new MemoryCache(
        new MemoryCacheOptions
        {
            TrackStatistics = true,
            SizeLimit = 1000,
        });
}