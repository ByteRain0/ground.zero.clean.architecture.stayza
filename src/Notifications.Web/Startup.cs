using Microsoft.Extensions.Diagnostics.HealthChecks;
using Notifications.Web.Greetings;
using Notifications.Web.Infrastructure.EndpointConfigs;
using Notifications.Web.Infrastructure.Startup;

namespace Notifications.Web;

public class Startup : BaseStartup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    
    public IConfiguration Configuration { get; }

    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" });

        
        services.AddScoped<IHelloService, RealHelloService>();
    }

    public override void Configure(IApplicationBuilder app)
    {
        app.MapEndpointsFrom<GreetingEndpoints>();
    }
}