using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using PactNet;
using PactNet.Output.Xunit;
using Stayza.Infrastructure.Notifications;
using Xunit.Abstractions;

namespace Stayza.Tests.Contract;

public class NotificationServiceContract
{
    private readonly IPactBuilderV4 _pactBuilder;

    public NotificationServiceContract(ITestOutputHelper output)
    {
        var config = new PactConfig
        {
            PactDir = "../../../../Notification-API-Pacts",
            Outputters = new[]
            {
                new XunitOutput(output) // Workaround xUnit to capture console output
            },
            DefaultJsonSettings = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            },
            LogLevel = PactLogLevel.Debug
        };

        var pact = Pact.V4(
            consumer: "Stayza.API",
            provider: "Notifications.API",
            config: config);
            
        _pactBuilder = pact.WithHttpInteractions();
    }

    [Fact]
    public async Task SendNotifications_WhenEmailAndMessageProvided_ReturnsOk()
    {
        // Arrange
        // Really similar to Wiremock -- we set up a mock API that will behave following a specific setup
        _pactBuilder
            .UponReceiving("A POST request to send notification")
            .Given("Type and UserId are provided")
            .WithRequest(HttpMethod.Post, "/api/v1/notifications")
            .WithBody("{\"type\":\"ReservationFulfilled\",\"userId\":\"bbb875a2-b427-4ee5-8957-e30343e108b6\"}","application/json")
            .WillRespond()
            .WithStatus(HttpStatusCode.Created); // For sake of test switch to an 200 response to show how it behaves.

        // Act / Assert
        await _pactBuilder.VerifyAsync(async ctx =>
        {
            var client = new UserNotificationsService(new HttpClient()
            {
                BaseAddress = ctx.MockServerUri
            });
            
            // Since this is a simple PoC the SendNotificationAsync will throw an exception in case something goes wrong
            // which will make the test fail. In more elaborate scenarios you might need to add additional assertions.
            await client.NotifyUser(userId:"bbb875a2-b427-4ee5-8957-e30343e108b6", eventType:"ReservationFulfilled");
        });
    }
}