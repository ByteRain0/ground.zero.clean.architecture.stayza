using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notifications.Web;
using Notifications.Web.Greetings;

namespace Notifications.Tests.Contract;

public class TestStartup : Startup
{
    public TestStartup(IConfiguration configuration) 
        : base(configuration)
    {
    }

    /// <summary>
    /// Provide overrides for the API services if needed.
    /// </summary>
    /// <param name="services"></param>
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddScoped<IHelloService, MockHelloService>();
    }
}