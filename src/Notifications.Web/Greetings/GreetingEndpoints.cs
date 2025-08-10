using Notifications.Web.Infrastructure.EndpointConfigs;

namespace Notifications.Web.Greetings;

internal class GreetingEndpoints : IEndpointsDefinition
{
    public static void ConfigureEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/hello", 
            (IHelloService helloService) => Results.Ok(helloService.GetHelloMessage()));
    }
}