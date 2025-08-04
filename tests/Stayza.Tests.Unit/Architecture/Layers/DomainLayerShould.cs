using static Stayza.Tests.Unit.Architecture.Constants;
namespace Stayza.Tests.Unit.Architecture.Layers;

public class DomainLayerShould : ArchitectureTestsBase
{
    [Fact]
    public void Not_have_dependency_on_Application_layer() 
        => EnsureTypesFromNamespaceDoNotDependOn(Namespaces.Domain, Namespaces.Application);

    [Fact]
    public void Not_have_dependency_on_Infrastructure_layer() 
        => EnsureTypesFromNamespaceDoNotDependOn(Namespaces.Domain, Namespaces.Infrastructure);
    
    [Fact]
    public void Not_have_dependency_on_Web_layer()
        => EnsureTypesFromNamespaceDoNotDependOn(Namespaces.Domain, Namespaces.Web);
}