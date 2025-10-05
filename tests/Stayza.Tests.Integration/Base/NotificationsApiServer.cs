using System.Text.Json;
using Stayza.Infrastructure.Notifications;
using WireMock.Client.Extensions;
using WireMock.Net.Testcontainers;

namespace Stayza.Tests.Integration.Base;

public class NotificationsApiServer : IAsyncDisposable
{
    private readonly WireMockContainer _notificationsApi = new WireMockContainerBuilder()
        .WithAutoRemove(true)
        .WithCleanUp(true)
        .Build();

    public string Url => _notificationsApi.GetPublicUrl();

    public async Task StartAsync()
    {
        await _notificationsApi.StartAsync();
    }

    public async Task SetUpNotificationResponse(bool successful)
    {
        var mappingBuilder = _notificationsApi
            .CreateWireMockAdminClient()
            .GetMappingBuilder();

        mappingBuilder.Given(builder => builder
            .WithRequest(req =>
                req.UsingPost()
                    .WithPath("/api/v1/notifications"))
            .WithResponse(rsp =>
                rsp.WithStatusCode(successful ? 200 : 400)));

        await mappingBuilder.BuildAndPostAsync();
    }

    public async Task<bool> CheckThatNotificationHasBeenReceived(
        string userId,
        string notificationType)
    {
        var adminClient = _notificationsApi.CreateWireMockAdminClient();
        var receivedRequests = await adminClient.GetRequestsAsync();

        var existingNotifications = new List<UserNotificationsService.Notification>();

        foreach (var receivedRequest in receivedRequests)
        {
            try
            {
                if (string.IsNullOrEmpty(receivedRequest.Request.Body))
                {
                    break;
                }

                var notification =
                    JsonSerializer.Deserialize<UserNotificationsService.Notification>(receivedRequest.Request.Body);

                existingNotifications.Add(notification);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not deserialize request into notification. Error : {e.Message}");
            }
        }

        return existingNotifications.Any(x =>
            x.UserId == userId && x.Type == notificationType);
    }

    public async ValueTask DisposeAsync()
    {
        await _notificationsApi.DisposeAsync();
    }
}