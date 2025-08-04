using static Stayza.Tests.Unit.Architecture.Constants;
namespace Stayza.Tests.Unit.Architecture.Layers;

public class ApplicationLayerShould : ArchitectureTestsBase
{
    [Fact]
    public void Not_have_dependency_on_Infrastructure_layer() 
        => EnsureTypesFromNamespaceDoNotDependOn(Namespaces.Application, Namespaces.Infrastructure);
    
    [Fact]
    public void Not_have_dependency_on_Web_layer()
        => EnsureTypesFromNamespaceDoNotDependOn(Namespaces.Application, Namespaces.Web);
}