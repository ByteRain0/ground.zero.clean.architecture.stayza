namespace Notifications.Web.Infrastructure.Startup;

public abstract class BaseStartup
{
    public abstract void ConfigureServices(IServiceCollection services);

    public abstract void Configure(IApplicationBuilder app);
}