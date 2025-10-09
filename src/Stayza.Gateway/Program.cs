using Microsoft.AspNetCore.RateLimiting;
using Yarp.ReverseProxy.LoadBalancing;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddSingleton<ILoadBalancingPolicy, TenantBasedLoadBalancingPolicy>()
    .AddRateLimiter(options =>
    {
        options.AddFixedWindowLimiter("fixedWindowPolicy", opt =>
        {
            opt.Window = TimeSpan.FromSeconds(20);
            opt.PermitLimit = 3;
        });
    })
    .AddReverseProxy()
    .AddCorrelationId()
    .LoadFromConfig(
        builder
            .Configuration
            .GetSection("ReverseProxy")
    );

var app = builder.Build();
app.MapReverseProxy();
app.UseRateLimiter();
app.Run();


#region Extensions

//   .AddCorrelationId()

#endregion