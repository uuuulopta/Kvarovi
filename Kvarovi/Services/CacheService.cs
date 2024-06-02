namespace Kvarovi.Services;

using Entities;
using Microsoft.Extensions.Caching.Memory;
using Repository;

public interface ICacheService
{
    ValueTask<int?> GetClientIdFromApiKey(string apiKey);
}

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IUserRepository _userRepository;

    public CacheService(ApiKeyUserMemoryCache memoryCache, IUserRepository clientsService)
    {
        _memoryCache = memoryCache.Cache;
        
        _userRepository = clientsService;
    }

    public async ValueTask<int?> GetClientIdFromApiKey(string apiKey)
    {
        if (!_memoryCache.TryGetValue<int>(apiKey, out var clientId))
        {
            User? user = await _userRepository.getUserByApiKeyAsync(apiKey);
            if (user != null)
            {
                var opt = new MemoryCacheEntryOptions();
                opt.Size = 1;
                opt.SlidingExpiration = TimeSpan.FromHours(1);
                _memoryCache.Set(apiKey, user.UserId,opt);
                
                clientId = user.UserId;
            }
            
            else return null;
        }

        return clientId;


    }
}
