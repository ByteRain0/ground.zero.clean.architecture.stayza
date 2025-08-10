using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Notifications.Web.Infrastructure.Startup;
using PactNet.Output.Xunit;
using PactNet.Verifier;
using Xunit.Abstractions;

namespace Notifications.Tests.Contract;

public class InstanceBasedNotificationsApiShould : IDisposable
{
    private ITestOutputHelper _outputHelper { get; }
    private IHost _api { get; }
    
    public InstanceBasedNotificationsApiShould(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _api = WebApplication
            .CreateBuilder()
            .Build<TestStartup>();
    }

    [Fact]
    public async Task HonorNotificationsConsumingPact()
    {
        await _api.StartAsync();

        // Arrange
        var config = new PactVerifierConfig
        {
            Outputters = new[]
            {
                new XunitOutput(_outputHelper),
            },
        };

        // Act / Assert
        using var pactVerifier = new PactVerifier("Notifications.API", config);

        pactVerifier
            .WithHttpEndpoint(new Uri("http://localhost:5000"))
            .WithFileSource(new FileInfo("../../../../Notification-API-Pacts/Stayza.API-Notifications.API.json"))
            .Verify();
    }

    public void Dispose()
    {
        _api.Dispose();
    }
}