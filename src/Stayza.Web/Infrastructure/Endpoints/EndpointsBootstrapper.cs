using System.Reflection;

namespace Stayza.Web.Infrastructure.Endpoints;

internal static class EndpointsBootstrapper
{
    internal static IApplicationBuilder MapEndpointsFrom<TMarker>(this IApplicationBuilder app)
    {
        MapEndpointsFrom(app, typeof(TMarker).Assembly);

        return app;
    }

    private static void MapEndpointsFrom(this IApplicationBuilder app, Assembly assembly)
    {
        var endpointTypes = GetEndpointDefinitionsFromAssembly(assembly);

        foreach (var endpointType in endpointTypes)
        {
            endpointType.GetMethod(nameof(IEndpointsDefinition.ConfigureEndpoints))!
                .Invoke(null, new object[]
                {
                    app
                });
        }
    }

    private static IEnumerable<TypeInfo> GetEndpointDefinitionsFromAssembly(Assembly assembly)
    {
        var endpointDefinitions =
            assembly
                .DefinedTypes
                .Where(x => x is
                {
                    IsAbstract: false,
                    IsInterface: false
                }
                            && typeof(IEndpointsDefinition).IsAssignableFrom(x));

        return endpointDefinitions;
    }
}