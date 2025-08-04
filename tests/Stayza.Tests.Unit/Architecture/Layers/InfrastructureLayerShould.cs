namespace Stayza.Tests.Unit.Architecture.Layers;

public class InfrastructureLayerShould : ArchitectureTestsBase
{
    [Fact]
    public void Not_have_dependency_on_Web_layer()
        => EnsureTypesFromNamespaceDoNotDependOn(Constants.Namespaces.Infrastructure, Constants.Namespaces.Web);
}