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
    

    public async ValueTask DisposeAsync()
    {
        await _notificationsApi.DisposeAsync();
    }
}