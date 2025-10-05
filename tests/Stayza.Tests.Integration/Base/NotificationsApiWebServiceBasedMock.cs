using System.Text.Json;
using Stayza.Infrastructure.Notifications;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Stayza.Tests.Integration.Base;

public class NotificationsApiWebServiceBasedMock : IDisposable
{
    public WireMockServer _wireMockServer;
    
    public string Url => _wireMockServer.Url;

    
    public NotificationsApiWebServiceBasedMock()
    {
        _wireMockServer = WireMockServer.Start();
    }

    public Task SetUpNotificationResponse(bool successful)
    {
        var objectResponse = new UserNotificationsService.Notification();
        
        _wireMockServer.Given(Request.Create()
                .WithPath("/api/v1/notifications")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithBody(JsonSerializer.Serialize(objectResponse))
                .WithHeader("content-type", "application/json; charset=utf-8")
                .WithStatusCode(successful ? 200 : 400));

        return Task.CompletedTask;
    }

    public Task<bool> CheckThatNotificationHasBeenReceived(
        string userId,
        string notificationType)
    {
        var test = _wireMockServer.LogEntries.ToList();
        return Task.FromResult(true);
    }
    
    
    public void Dispose()
    {
        _wireMockServer.Stop();
        _wireMockServer.Dispose();
    }
}