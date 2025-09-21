using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using Stayza.Domain.Books;

namespace Stayza.Infrastructure.Cache;

public static class ApplicationLayerCacheApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddInfrastructureCache(this IHostApplicationBuilder builder)
    {
        var cacheSettings = builder
            .Configuration
            .GetSection("Cache")
            .Get<CacheSettings>()!;

        if (!cacheSettings.Enabled)
        {
            return builder;
        }

        #region Redis cache setup

        builder.Services.AddStackExchangeRedisCache(opts =>
        {
            opts.Configuration = cacheSettings.ConnectionString;
        });
        
        #endregion
        
        #region Hybrid Cache setup

        builder.Services.AddHybridCache(opts =>
        {
            opts.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(1),
                Expiration = TimeSpan.FromMinutes(10)
            };
            
        });

        #endregion
        
        #region Output cache setup

        // Set up cache policies at API level
        // builder.Services.AddOutputCache(opts =>
        // {
        //     // Cache by default everything
        //     opts.AddBasePolicy(policy => policy.Cache());
        //     
        //     opts.AddPolicy("GetLoansByUserId", policy =>
        //     {
        //         policy.Cache()
        //             .Expire(TimeSpan.FromMinutes(1))
        //             // A bit of a bad example since we are not passing the userId in here and it will cache randomly.
        //             .SetVaryByHeader("page", "pageSize", "sortColumn", "sortOrder")
        //             .Tag("getloansbyuserid");
        //     });
        // });

        // If you want redis in there.
        // builder.Services.AddStackExchangeRedisOutputCache(opts =>
        // {
        //     opts.Configuration = cacheSettings.ConnectionString;
        // });
        
        #endregion

        
        #region Add cache to repositories

        builder.Services.Decorate<IBooksRepository, BookRepositoryCache>();
        
        #endregion
        
        return builder;
    }
}