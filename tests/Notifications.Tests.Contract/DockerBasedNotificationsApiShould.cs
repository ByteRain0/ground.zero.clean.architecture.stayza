using DotNet.Testcontainers.Builders;
using PactNet.Output.Xunit;
using PactNet.Verifier;
using Xunit.Abstractions;

namespace Notifications.Tests.Contract;

/// <summary>
/// Assuming you can generate a container image of your API you can technically test it like this.
/// </summary>
public class DockerBasedNotificationsApiShould
{
    private ITestOutputHelper _outputHelper { get; }

    public DockerBasedNotificationsApiShould(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }


    [Fact(Skip = "Ignored in favour of building the api manually")]
    public async Task HonorNotificationsConsumingPact()
    {
        // Arrange
        var container = new ContainerBuilder()
            .WithImage("notifications.api")
            .WithPortBinding(9055, 80)
            .WithEnvironment("ASPNETCORE_URLS","http://+:80")
            .Build();
        
        await container.StartAsync();
        
        var config = new PactVerifierConfig
        {
            Outputters = new[]
            {
                new XunitOutput(_outputHelper)
            }
        };
        
        // Act / Assert
        using var pactVerifier = new PactVerifier("Notifications.API", config);

        pactVerifier
            .WithHttpEndpoint(new Uri($"http://{container.Hostname}:9055"))
            .WithFileSource(new FileInfo("../../../../Notification-API-Pacts/Stayza.API-Notifications.API.json"))
            .Verify();
    }
}