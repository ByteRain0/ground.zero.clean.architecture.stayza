using Microsoft.AspNetCore.Builder;
using Notifications.Web.Infrastructure.Startup;
using PactNet.Output.Xunit;
using PactNet.Verifier;
using Xunit.Abstractions;

namespace Notifications.Tests.Contract;

public class InstanceBasedNotificationsApiShould : IAsyncDisposable
{
    private ITestOutputHelper _outputHelper { get; }
    private WebApplication _api { get; }

    private static string _localhostAddress = "http://localhost:5010";
    
    public InstanceBasedNotificationsApiShould(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _api = WebApplication
            .CreateBuilder()
            .Build<TestStartup>();
        
        _api.Urls.Add(_localhostAddress);
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
            }
        };

        // Act / Assert
        using var pactVerifier = new PactVerifier("Notifications.API", config);

        pactVerifier
            .WithHttpEndpoint(new Uri(_localhostAddress))
            .WithFileSource(new FileInfo("../../../../Notification-API-Pacts/Stayza.API-Notifications.API.json"))
            .Verify();
    }

    public async ValueTask DisposeAsync()
    {
        await _api.DisposeAsync();
    }
}