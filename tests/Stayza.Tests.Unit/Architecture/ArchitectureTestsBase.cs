using NetArchTest.Rules;
using Shouldly;
using Stayza.Web;

namespace Stayza.Tests.Unit.Architecture;

public class ArchitectureTestsBase
{
    private PredicateList GetTypesFrom(string _namespace) => Types
        .InAssembly(typeof(IWebMarker).Assembly)
        .That().ResideInNamespace(_namespace);
    
    protected void EnsureTypesFromNamespaceDoNotDependOn(string nameSpace, string dependencyNamespace)
    {
        GetTypesFrom(nameSpace)
            .Should()
            .NotHaveDependencyOnAny(GetTypesFrom(dependencyNamespace).GetTypes().Select(x => x.FullName).ToArray())
            .GetResult()
            .IsSuccessful
            .ShouldBeTrue();
    }
}