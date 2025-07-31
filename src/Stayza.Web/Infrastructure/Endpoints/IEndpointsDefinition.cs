namespace Stayza.Web.Infrastructure.Endpoints;

internal interface IEndpointsDefinition
{
    public static abstract void ConfigureEndpoints(IEndpointRouteBuilder app);
}