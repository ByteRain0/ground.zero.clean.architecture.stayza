namespace Notifications.Web.Infrastructure.EndpointConfigs;

internal interface IEndpointsDefinition
{
    public static abstract void ConfigureEndpoints(IEndpointRouteBuilder app);
}