using Microsoft.Extensions.Caching.Distributed;

namespace Stayza.Infrastructure.Cache;

public class CacheService  
{  
    private readonly IDistributedCache _cache;  
  
    // Decision: Using Redis as our distributed cache system.  
    // Reason: Redis offers low-latency caching, which is essential for our application's performance requirements.  
    // Alternatives: In-memory caching (not distributed), Memcached were considered.  
    // Consequences: Need to manage Redis infrastructure, but performance benefits outweigh this cost.  
    public CacheService(IDistributedCache cache)  
    {  
        _cache = cache;  
    }  
  
    public async Task<string> GetCachedValueAsync(string key)  
    {  
        return await _cache.GetStringAsync(key);  
    }  
}