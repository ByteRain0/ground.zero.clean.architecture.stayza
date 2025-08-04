using NetArchTest.Rules;
using Shouldly;
using Stayza.Web;
using Stayza.Web.Infrastructure.Endpoints;

namespace Stayza.Tests.Unit.Architecture.Design;

public class EndpointDefinitionsShould
{
    [Fact]
    public void Not_be_public()
    {
        Types.InAssembly(typeof(IWebMarker).Assembly)
            .That().ImplementInterface(typeof(IEndpointsDefinition))
            .Should().NotBePublic()
            .GetResult()
            .IsSuccessful.ShouldBeTrue();
    }
    
    [Fact]
    public void End_with_Endpoints()
    {
        Types.InAssembly(typeof(IWebMarker).Assembly)
            .That().ImplementInterface(typeof(IEndpointsDefinition))
            .Should().HaveNameEndingWith("Endpoints")
            .GetResult()
            .IsSuccessful.ShouldBeTrue();
    }
}